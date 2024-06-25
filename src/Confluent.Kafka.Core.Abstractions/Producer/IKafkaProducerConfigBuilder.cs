using System;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerConfigBuilder : IProducerConfigBuilder<IKafkaProducerConfigBuilder>, IDisposable
    {
        IKafkaProducerConfigBuilder FromConfiguration(string sectionKey);

        IKafkaProducerConfigBuilder WithDefaultTopic(string defaultTopic);

        IKafkaProducerConfigBuilder WithDefaultPartition(Partition defaultPartition);

        IKafkaProducerConfigBuilder WithDefaultTimeout(TimeSpan defaultTimeout);

        IKafkaProducerConfigBuilder WithPollAfterProducing(bool pollAfterProducing);

        IKafkaProducerConfigBuilder WithEnableLogging(bool enableLogging);

        IKafkaProducerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics);

        IKafkaProducerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure);

        IKafkaProducerConfigBuilder WithEnableInterceptorExceptionPropagation(bool enableInterceptorExceptionPropagation);

        IKafkaProducerConfig Build();
    }
}
