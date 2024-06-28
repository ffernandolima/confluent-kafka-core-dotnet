using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal sealed class KafkaConsumerWorkerConfigBuilder :
        FunctionalBuilder<KafkaConsumerWorkerConfig, IKafkaConsumerWorkerConfig, KafkaConsumerWorkerConfigBuilder>,
        IKafkaConsumerWorkerConfigBuilder
    {
        public KafkaConsumerWorkerConfigBuilder(IKafkaConsumerWorkerConfig workerConfig = null, IConfiguration configuration = null)
            : base(workerConfig, configuration)
        { }

        public IKafkaConsumerWorkerConfigBuilder FromConfiguration(string sectionKey)
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

        public IKafkaConsumerWorkerConfigBuilder WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
        {
            AppendAction(config => config.MaxDegreeOfParallelism = maxDegreeOfParallelism);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(config => config.EnableLogging = enableLogging);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics)
        {
            AppendAction(config => config.EnableDiagnostics = enableDiagnostics);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithCommitFaultedMessages(bool commitFaultedMessages)
        {
            AppendAction(config => config.CommitFaultedMessages = commitFaultedMessages);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithEnableIdempotency(bool enableIdempotency)
        {
            AppendAction(config => config.EnableIdempotency = enableIdempotency);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure)
        {
            AppendAction(config => config.EnableRetryOnFailure = enableRetryOnFailure);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithEnableRetryTopic(bool enableRetryTopic)
        {
            AppendAction(config => config.EnableRetryTopic = enableRetryTopic);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithRetryTopicExceptionTypeFilters(string[] retryTopicExceptionTypeFilters)
        {
            AppendAction(config => config.RetryTopicExceptionTypeFilters = retryTopicExceptionTypeFilters);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithRetryTopicExceptionFilter(Func<Exception, bool> retryTopicExceptionFilter)
        {
            AppendAction(config => config.RetryTopicExceptionFilter = retryTopicExceptionFilter);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithEnableDeadLetterTopic(bool enableDeadLetterTopic)
        {
            AppendAction(config => config.EnableDeadLetterTopic = enableDeadLetterTopic);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithEmptyTopicDelay(TimeSpan emptyTopicDelay)
        {
            AppendAction(config => config.EmptyTopicDelay = emptyTopicDelay);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithNotEmptyTopicDelay(TimeSpan notEmptyTopicDelay)
        {
            AppendAction(config => config.NotEmptyTopicDelay = notEmptyTopicDelay);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithUnavailableProcessingSlotsDelay(TimeSpan unavailableProcessingSlotsDelay)
        {
            AppendAction(config => config.UnavailableProcessingSlotsDelay = unavailableProcessingSlotsDelay);
            return this;
        }

        public IKafkaConsumerWorkerConfigBuilder WithPendingProcessingDelay(TimeSpan pendingProcessingDelay)
        {
            AppendAction(config => config.PendingProcessingDelay = pendingProcessingDelay);
            return this;
        }

        public static IKafkaConsumerWorkerConfig BuildConfig(
           IConfiguration configuration = null,
           IKafkaConsumerWorkerConfig workerConfig = null,
           Action<IKafkaConsumerWorkerConfigBuilder> configureWorker = null)
        {
            using var builder = new KafkaConsumerWorkerConfigBuilder(workerConfig, configuration);

            configureWorker?.Invoke(builder);

            return builder.Build();
        }
    }
}
