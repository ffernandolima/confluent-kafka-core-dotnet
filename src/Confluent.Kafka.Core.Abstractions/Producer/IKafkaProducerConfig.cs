using System;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerConfig : IProducerConfig, IValidatableObject
    {
        TimeSpan DefaultTimeout { get; }

        bool EnableLogging { get; }

        bool EnableDiagnostics { get; }

        bool EnableRetryOnFailure { get; }

        bool EnableInterceptorExceptionPropagation { get; }
    }
}
