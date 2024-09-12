using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Hosting.Retry.Internal
{
    internal sealed class KafkaRetryConsumerWorkerConfigBuilder :
        FunctionalBuilder<KafkaRetryConsumerWorkerConfig, IKafkaRetryConsumerWorkerConfig, KafkaRetryConsumerWorkerConfigBuilder>,
        IKafkaRetryConsumerWorkerConfigBuilder
    {
        public KafkaRetryConsumerWorkerConfigBuilder(IKafkaRetryConsumerWorkerConfig workerConfig = null, IConfiguration configuration = null)
            : base(workerConfig, configuration)
        { }

        public IKafkaRetryConsumerWorkerConfigBuilder FromConfiguration(string sectionKey)
        {
            AppendAction(config =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    config = Bind(config, sectionKey);
                }
            });
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
        {
            AppendAction(config => config.MaxDegreeOfParallelism = maxDegreeOfParallelism);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(config => config.EnableLogging = enableLogging);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics)
        {
            AppendAction(config => config.EnableDiagnostics = enableDiagnostics);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithCommitFaultedMessages(bool commitFaultedMessages)
        {
            AppendAction(config => config.CommitFaultedMessages = commitFaultedMessages);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithEnableIdempotency(bool enableIdempotency)
        {
            AppendAction(config => config.EnableIdempotency = enableIdempotency);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure)
        {
            AppendAction(config => config.EnableRetryOnFailure = enableRetryOnFailure);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithEnableDeadLetterTopic(bool enableDeadLetterTopic)
        {
            AppendAction(config => config.EnableDeadLetterTopic = enableDeadLetterTopic);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithRetryCount(int retryCount)
        {
            AppendAction(config => config.RetryCount = retryCount);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithEmptyTopicDelay(TimeSpan emptyTopicDelay)
        {
            AppendAction(config => config.EmptyTopicDelay = emptyTopicDelay);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithNotEmptyTopicDelay(TimeSpan notEmptyTopicDelay)
        {
            AppendAction(config => config.NotEmptyTopicDelay = notEmptyTopicDelay);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithUnavailableProcessingSlotsDelay(TimeSpan unavailableProcessingSlotsDelay)
        {
            AppendAction(config => config.UnavailableProcessingSlotsDelay = unavailableProcessingSlotsDelay);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithExceptionDelay(TimeSpan exceptionDelay)
        {
            AppendAction(config => config.ExceptionDelay = exceptionDelay);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithPendingProcessingDelay(TimeSpan pendingProcessingDelay)
        {
            AppendAction(config => config.PendingProcessingDelay = pendingProcessingDelay);
            return this;
        }

        public IKafkaRetryConsumerWorkerConfigBuilder WithRetryTopicDelay(TimeSpan retryTopicDelay)
        {
            AppendAction(config => config.RetryTopicDelay = retryTopicDelay);
            return this;
        }

        public static IKafkaRetryConsumerWorkerConfig BuildConfig(
           IConfiguration configuration = null,
           IKafkaRetryConsumerWorkerConfig workerConfig = null,
           Action<IKafkaRetryConsumerWorkerConfigBuilder> configureWorker = null)
        {
            using var builder = new KafkaRetryConsumerWorkerConfigBuilder(workerConfig, configuration);

            configureWorker?.Invoke(builder);

            return builder.Build();
        }
    }
}
