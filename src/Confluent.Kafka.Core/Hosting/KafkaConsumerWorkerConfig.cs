using Confluent.Kafka.Core.Idempotency.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerWorkerConfig : IKafkaConsumerWorkerConfig
    {
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
        public TimeSpan EmptyTopicDelay { get; set; } = new TimeSpan(0, 0, 10);
        public TimeSpan NotEmptyTopicDelay { get; set; } = new TimeSpan(0, 0, 1);
        public TimeSpan UnavailableProcessingSlotsDelay { get; set; } = new TimeSpan(0, 0, 2);
        public TimeSpan PendingProcessingDelay { get; set; } = new TimeSpan(0, 0, 1);

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IKafkaConsumerWorkerConfig workerConfig = validationContext?.ObjectInstance as KafkaConsumerWorkerConfig ?? this;

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
            }
        }

        #endregion IValidatableObject Members
    }
}
