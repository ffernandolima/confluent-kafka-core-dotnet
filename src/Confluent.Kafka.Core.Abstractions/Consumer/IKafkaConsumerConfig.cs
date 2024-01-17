using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerConfig : IConsumerConfig, IValidatableObject
    {

        IEnumerable<string> TopicSubscriptions { get; }

        IEnumerable<TopicPartition> PartitionAssignments { get; }

        bool CommitAfterConsuming { get; }

        TimeSpan DefaultTimeout { get; }

        int DefaultBatchSize { get; }

        bool EnableLogging { get; }

        bool EnableDiagnostics { get; }

        bool EnableDeadLetterTopic { get; }

        bool EnableRetryOnFailure { get; }

        bool EnableInterceptorExceptionPropagation { get; }
    }
}