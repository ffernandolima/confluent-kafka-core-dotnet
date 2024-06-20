using System;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IKafkaConsumerWorkerConfig : IValidatableObject
    {
        int MaxDegreeOfParallelism { get; }

        bool EnableLogging { get; }

        bool EnableDiagnostics { get; }

        bool CommitFaultedMessages { get; }

        bool EnableIdempotency { get; }

        bool EnableRetryOnFailure { get; }

        bool EnableRetryTopic { get; }

        string[] RetryTopicExceptionTypeFilters { get; }

        Func<Exception, bool> RetryTopicExceptionFilter { get; }

        bool EnableDeadLetterTopic { get; }

        TimeSpan EmptyTopicDelay { get; }

        TimeSpan NotEmptyTopicDelay { get; }

        TimeSpan UnavailableProcessingSlotsDelay { get; }

        TimeSpan PendingProcessingDelay { get; }
    }
}
