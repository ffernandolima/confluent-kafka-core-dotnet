using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerConfigBuilder : IConsumerConfigBuilder<IKafkaConsumerConfigBuilder>, IDisposable
    {
        IKafkaConsumerConfigBuilder WithTopicSubscriptions(IEnumerable<string> topicSubscriptions);

        IKafkaConsumerConfigBuilder WithPartitionAssignments(IEnumerable<TopicPartition> partitionAssignments);

        IKafkaConsumerConfigBuilder WithCommitAfterConsuming(bool commitAfterConsuming);

        IKafkaConsumerConfigBuilder WithDefaultTimeout(TimeSpan defaultTimeout);

        IKafkaConsumerConfigBuilder WithDefaultBatchSize(int defaultBatchSize);

        IKafkaConsumerConfigBuilder WithEnableLogging(bool enableLogging);

        IKafkaConsumerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics);

        IKafkaConsumerConfigBuilder WithEnableDeadLetterTopic(bool enableDeadLetterTopic);

        IKafkaConsumerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure);

        IKafkaConsumerConfigBuilder WithEnableInterceptorExceptionPropagation(bool enableInterceptorExceptionPropagation);

        IKafkaConsumerConfig Build();
    }
}
