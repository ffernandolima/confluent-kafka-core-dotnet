using System;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public interface IKafkaRetryConsumerWorkerConfig : IValidatableObject
    {
        int MaxDegreeOfParallelism { get; }

        bool EnableLogging { get; }

        bool EnableDiagnostics { get; }

        bool CommitFaultedMessages { get; }

        bool EnableIdempotency { get; }

        bool EnableRetryOnFailure { get; }

        bool EnableDeadLetterTopic { get; }

        int RetryCount { get; }

        TimeSpan EmptyTopicDelay { get; }

        TimeSpan NotEmptyTopicDelay { get; }

        TimeSpan UnavailableProcessingSlotsDelay { get; }

        TimeSpan ExceptionDelay { get; }

        TimeSpan PendingProcessingDelay { get; }

        TimeSpan RetryTopicDelay { get; }
    }
}
