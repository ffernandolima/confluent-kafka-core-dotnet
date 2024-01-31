using System;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerConfig : IProducerConfig, IValidatableObject
    {
        string DefaultTopic { get; }

        Partition DefaultPartition { get; }

        TimeSpan DefaultTimeout { get; }

        bool PollAfterProducing { get; }

        bool EnableLogging { get; }

        bool EnableDiagnostics { get; }

        bool EnableRetryOnFailure { get; }

        bool EnableInterceptorExceptionPropagation { get; }
    }
}
