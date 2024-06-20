using System;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IKafkaConsumerWorkerConfigBuilder
    {
        IKafkaConsumerWorkerConfigBuilder WithMaxDegreeOfParallelism(int maxDegreeOfParallelism);

        IKafkaConsumerWorkerConfigBuilder WithEnableLogging(bool enableLogging);

        IKafkaConsumerWorkerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics);

        IKafkaConsumerWorkerConfigBuilder WithCommitFaultedMessages(bool commitFaultedMessages);

        IKafkaConsumerWorkerConfigBuilder WithEnableIdempotency(bool enableIdempotency);

        IKafkaConsumerWorkerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure);

        IKafkaConsumerWorkerConfigBuilder WithEnableRetryTopic(bool enableRetryTopic);

        IKafkaConsumerWorkerConfigBuilder WithRetryTopicExceptionTypeFilters(string[] retryTopicExceptionTypeFilters);

        IKafkaConsumerWorkerConfigBuilder WithRetryTopicExceptionFilter(Func<Exception, bool> retryTopicExceptionFilter);

        IKafkaConsumerWorkerConfigBuilder WithEnableDeadLetterTopic(bool enableDeadLetterTopic);

        IKafkaConsumerWorkerConfigBuilder WithEmptyTopicDelay(TimeSpan emptyTopicDelay);

        IKafkaConsumerWorkerConfigBuilder WithNotEmptyTopicDelay(TimeSpan notEmptyTopicDelay);

        IKafkaConsumerWorkerConfigBuilder WithUnavailableProcessingSlotsDelay(TimeSpan unavailableProcessingSlotsDelay);

        IKafkaConsumerWorkerConfigBuilder WithPendingProcessingDelay(TimeSpan pendingProcessingDelay);
    }
}
