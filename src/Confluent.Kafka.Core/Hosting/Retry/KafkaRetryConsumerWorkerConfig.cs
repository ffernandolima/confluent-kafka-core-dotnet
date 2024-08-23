using Confluent.Kafka.Core.Hosting.Retry.Internal;
using Confluent.Kafka.Core.Idempotency.Internal;
using Confluent.Kafka.Core.Mapping;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public sealed class KafkaRetryConsumerWorkerConfig : IKafkaRetryConsumerWorkerConfig, IMapper<IKafkaConsumerWorkerConfig>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumerWorkerConfig _mappedConfig;

        #region IKafkaRetryConsumerWorkerConfig Members

        public int MaxDegreeOfParallelism { get; set; } = 1;
        public bool EnableLogging { get; set; } = true;
        public bool EnableDiagnostics { get; set; } = true;
        public bool CommitFaultedMessages { get; set; }
        public bool EnableIdempotency { get; set; }
        public bool EnableRetryOnFailure { get; set; }
        public bool EnableDeadLetterTopic { get; set; }
        public int RetryCount { get; set; } = 1;
        public TimeSpan EmptyTopicDelay { get; set; } = new TimeSpan(0, 0, 5);
        public TimeSpan NotEmptyTopicDelay { get; set; } = new TimeSpan(0, 0, 1);
        public TimeSpan UnavailableProcessingSlotsDelay { get; set; } = new TimeSpan(0, 0, 2);
        public TimeSpan PendingProcessingDelay { get; set; } = new TimeSpan(0, 0, 1);
        public TimeSpan RetryTopicDelay { get; set; } = new TimeSpan(0, 1, 0);

        #endregion IKafkaRetryConsumerWorkerConfig Members

        #region IMapper Explicity Members

        IKafkaConsumerWorkerConfig IMapper<IKafkaConsumerWorkerConfig>.Map(params object[] args)
        {
            _mappedConfig ??= new KafkaConsumerWorkerConfig
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                EnableLogging = EnableLogging,
                EnableDiagnostics = EnableDiagnostics,
                CommitFaultedMessages = CommitFaultedMessages,
                EnableIdempotency = EnableIdempotency,
                EnableRetryOnFailure = EnableRetryOnFailure,
                EnableDeadLetterTopic = EnableDeadLetterTopic,
                EmptyTopicDelay = EmptyTopicDelay,
                NotEmptyTopicDelay = NotEmptyTopicDelay,
                UnavailableProcessingSlotsDelay = UnavailableProcessingSlotsDelay,
                PendingProcessingDelay = PendingProcessingDelay
            };

            return _mappedConfig;
        }

        #endregion IMapper Explicity Members

        #region Public Methods

        public static IKafkaRetryConsumerWorkerConfigBuilder CreateBuilder(IConfiguration configuration = null)
            => new KafkaRetryConsumerWorkerConfigBuilder(configuration: configuration);

        #endregion Public Methods


        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IKafkaRetryConsumerWorkerConfig workerConfig = validationContext?.ObjectInstance as KafkaRetryConsumerWorkerConfig ?? this;

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

                if (workerConfig.EnableDeadLetterTopic &&
                    validationContext.Items.TryGetValue(KafkaProducerConstants.DeadLetterProducer, out object deadLetterProducer) &&
                    deadLetterProducer is null)
                {
                    yield return new ValidationResult(
                        $"{KafkaProducerConstants.DeadLetterProducer} cannot be null when {nameof(workerConfig.EnableDeadLetterTopic)} is enabled.",
                        [KafkaProducerConstants.DeadLetterProducer, nameof(workerConfig.EnableDeadLetterTopic)]);
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

            if (workerConfig.PendingProcessingDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.PendingProcessingDelay)} cannot be infinite.",
                    [nameof(workerConfig.PendingProcessingDelay)]);
            }

            if (workerConfig.RetryTopicDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(workerConfig.RetryTopicDelay)} cannot be infinite.",
                    [nameof(workerConfig.RetryTopicDelay)]);
            }
        }

        #endregion IValidatableObject Members
    }
}
