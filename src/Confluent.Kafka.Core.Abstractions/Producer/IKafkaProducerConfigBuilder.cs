using System;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerConfigBuilder : IProducerConfigBuilder<IKafkaProducerConfigBuilder>, IDisposable
    {
        IKafkaProducerConfigBuilder WithDefaultTopic(string defaultTopic);

        IKafkaProducerConfigBuilder WithPollAfterProducing(bool pollAfterProducing);

        IKafkaProducerConfigBuilder WithDefaultTimeout(TimeSpan defaultTimeout);

        IKafkaProducerConfigBuilder WithEnableLogging(bool enableLogging);

        IKafkaProducerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics);

        IKafkaProducerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure);

        IKafkaProducerConfigBuilder WithEnableInterceptorExceptionPropagation(bool enableInterceptorExceptionPropagation);

        IKafkaProducerConfig Build();
    }
}
