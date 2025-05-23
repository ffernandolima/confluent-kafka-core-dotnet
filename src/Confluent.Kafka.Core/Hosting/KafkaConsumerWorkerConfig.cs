﻿using Confluent.Kafka.Core.Hosting.Internal;
using Confluent.Kafka.Core.Idempotency.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry;
using Confluent.Kafka.Core.Retry.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerWorkerConfig : IKafkaConsumerWorkerConfig
    {
        private RetrySpecification _retrySpecification;

        #region IKafkaConsumerWorkerConfig Members

        public int MaxDegreeOfParallelism { get; set; } = 1;
        public bool EnableLogging { get; set; } = true;
        public bool EnableDiagnostics { get; set; } = true;
        public bool CommitFaultedMessages { get; set; }
        public bool EnableIdempotency { get; set; }
        public bool EnableRetryOnFailure { get; set; }
        public bool EnableRetryTopic { get; set; }
        public string[] RetryTopicExceptionTypeFilters { get; set; }
        public Func<Exception, bool> RetryTopicExceptionFilter { get; set; }
        public bool EnableDeadLetterTopic { get; set; }
        public bool EnableMessageOrderGuarantee { get; set; }
        public TimeSpan EmptyTopicDelay { get; set; } = new TimeSpan(0, 0, 3);
        public TimeSpan NotEmptyTopicDelay { get; set; } = new TimeSpan(0, 0, 1);
        public TimeSpan UnavailableProcessingSlotsDelay { get; set; } = new TimeSpan(0, 0, 2);
        public TimeSpan ExceptionDelay { get; set; } = new TimeSpan(0, 0, 5);
        public TimeSpan PendingProcessingDelay { get; set; } = new TimeSpan(0, 0, 1);
        public RetrySpecification RetryTopicSpecification =>
            _retrySpecification ??= RetrySpecification.Create(RetryTopicExceptionFilter, RetryTopicExceptionTypeFilters);

        #endregion IKafkaConsumerWorkerConfig Members

        #region Public Methods

        public static IKafkaConsumerWorkerConfigBuilder CreateBuilder(IConfiguration configuration = null)
            => new KafkaConsumerWorkerConfigBuilder(configuration: configuration);

        #endregion Public Methods

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IKafkaConsumerWorkerConfig workerConfig = validationContext?.ObjectInstance as KafkaConsumerWorkerConfig ?? this;

            if (validationContext?.Items is not null)
            {
                if (workerConfig.EnableIdempotency &&
                    validationContext.Items.TryGetValue(KafkaIdempotencyConstants.IdempotencyHandler, out object idempotencyHandler) &&
                    idempotencyHandler is null)
                {
                    yield return new ValidationResult(
                        $"{KafkaIdempotencyConstants.IdempotencyHandler} cannot be null when {nameof(workerConfig.EnableIdempotency)} is enabled.",
                        [KafkaIdempotencyConstants.IdempotencyHandler, nameof(workerConfig.EnableIdempotency)]);
                }

                if (workerConfig.EnableRetryOnFailure &&
                    validationContext.Items.TryGetValue(KafkaRetryConstants.RetryHandler, out object retryHandler) &&
                    retryHandler is null)
                {
                    yield return new ValidationResult(
                        $"{KafkaRetryConstants.RetryHandler} cannot be null when {nameof(workerConfig.EnableRetryOnFailure)} is enabled.",
                        [KafkaRetryConstants.RetryHandler, nameof(workerConfig.EnableRetryOnFailure)]);
                }

                if (workerConfig.EnableRetryTopic &&
                    validationContext.Items.TryGetValue(KafkaProducerConstants.RetryProducer, out object retryProducer) &&
                    retryProducer is null)
                {
                    yield return new ValidationResult(
                        $"{KafkaProducerConstants.RetryProducer} cannot be null when {nameof(workerConfig.EnableRetryTopic)} is enabled.",
                        [KafkaProducerConstants.RetryProducer, nameof(workerConfig.EnableRetryTopic)]);
                }

                if (workerConfig.EnableDeadLetterTopic &&
                    validationContext.Items.TryGetValue(KafkaProducerConstants.DeadLetterProducer, out object deadLetterProducer) &&
                    deadLetterProducer is null)
                {
                    yield return new ValidationResult(
                        $"{KafkaProducerConstants.DeadLetterProducer} cannot be null when {nameof(workerConfig.EnableDeadLetterTopic)} is enabled.",
                        [KafkaProducerConstants.DeadLetterProducer, nameof(workerConfig.EnableDeadLetterTopic)]);
                }

                if (workerConfig.EnableMessageOrderGuarantee &&
                    validationContext.Items.TryGetValue(MessageOrderGuaranteeConstants.MessageOrderGuaranteeKeyHandler, out object messageOrderGuaranteeKeyHandler) &&
                    messageOrderGuaranteeKeyHandler is null)
                {
                    yield return new ValidationResult(
                        $"{MessageOrderGuaranteeConstants.MessageOrderGuaranteeKeyHandler} cannot be null when {nameof(workerConfig.EnableMessageOrderGuarantee)} is enabled.",
                        [MessageOrderGuaranteeConstants.MessageOrderGuaranteeKeyHandler, nameof(workerConfig.EnableMessageOrderGuarantee)]);
                }
            }

            if (workerConfig.MaxDegreeOfParallelism <= 0)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.MaxDegreeOfParallelism)} cannot be less than or equal to zero.",
                    [nameof(workerConfig.MaxDegreeOfParallelism)]);
            }

            if (workerConfig.EmptyTopicDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.EmptyTopicDelay)} cannot be infinite.",
                    [nameof(workerConfig.EmptyTopicDelay)]);
            }

            if (workerConfig.NotEmptyTopicDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.NotEmptyTopicDelay)} cannot be infinite.",
                    [nameof(workerConfig.NotEmptyTopicDelay)]);
            }

            if (workerConfig.UnavailableProcessingSlotsDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.UnavailableProcessingSlotsDelay)} cannot be infinite.",
                    [nameof(workerConfig.UnavailableProcessingSlotsDelay)]);
            }

            if (workerConfig.ExceptionDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.ExceptionDelay)} cannot be infinite.",
                    [nameof(workerConfig.ExceptionDelay)]);
            }

            if (workerConfig.PendingProcessingDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.PendingProcessingDelay)} cannot be infinite.",
                    [nameof(workerConfig.PendingProcessingDelay)]);
            }
        }

        #endregion IValidatableObject Members
    }
}
