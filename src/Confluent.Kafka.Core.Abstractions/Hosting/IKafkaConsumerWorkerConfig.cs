using Confluent.Kafka.Core.Retry;
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

        RetrySpecification RetryTopicSpecification { get; }

        bool EnableDeadLetterTopic { get; }

        bool EnableMessageOrderGuarantee { get; }

        TimeSpan EmptyTopicDelay { get; }

        TimeSpan NotEmptyTopicDelay { get; }

        TimeSpan UnavailableProcessingSlotsDelay { get; }

        TimeSpan ExceptionDelay { get; }

        TimeSpan PendingProcessingDelay { get; }
    }
}
