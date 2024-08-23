using System;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public interface IKafkaRetryConsumerWorkerConfigBuilder
    {
        IKafkaRetryConsumerWorkerConfigBuilder FromConfiguration(string sectionKey);

        IKafkaRetryConsumerWorkerConfigBuilder WithMaxDegreeOfParallelism(int maxDegreeOfParallelism);

        IKafkaRetryConsumerWorkerConfigBuilder WithEnableLogging(bool enableLogging);

        IKafkaRetryConsumerWorkerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics);

        IKafkaRetryConsumerWorkerConfigBuilder WithCommitFaultedMessages(bool commitFaultedMessages);

        IKafkaRetryConsumerWorkerConfigBuilder WithEnableIdempotency(bool enableIdempotency);

        IKafkaRetryConsumerWorkerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure);

        IKafkaRetryConsumerWorkerConfigBuilder WithEnableDeadLetterTopic(bool enableDeadLetterTopic);

        IKafkaRetryConsumerWorkerConfigBuilder WithRetryCount(int retryCount);

        IKafkaRetryConsumerWorkerConfigBuilder WithEmptyTopicDelay(TimeSpan emptyTopicDelay);

        IKafkaRetryConsumerWorkerConfigBuilder WithNotEmptyTopicDelay(TimeSpan notEmptyTopicDelay);

        IKafkaRetryConsumerWorkerConfigBuilder WithUnavailableProcessingSlotsDelay(TimeSpan unavailableProcessingSlotsDelay);

        IKafkaRetryConsumerWorkerConfigBuilder WithPendingProcessingDelay(TimeSpan pendingProcessingDelay);

        IKafkaRetryConsumerWorkerConfigBuilder WithRetryTopicDelay(TimeSpan retryTopicDelay);

        IKafkaRetryConsumerWorkerConfig Build();
    }
}
