using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Confluent.Kafka.Core.Consumer
{
    public class KafkaConsumerConfig : ConsumerConfig, IKafkaConsumerConfig, IKafkaConsumerConfigHandler
    {
        #region Ctors

        public KafkaConsumerConfig()
            : base()
        { }

        public KafkaConsumerConfig(ConsumerConfig config)
            : base(config)
        { }

        public KafkaConsumerConfig(ClientConfig config)
           : base(config)
        { }

        public KafkaConsumerConfig(IDictionary<string, string> config)
            : base(config)
        { }

        #endregion Ctors

        #region Hidden Inherited Members

        private int _cancellationDelayMaxMs = 100;
        public new int CancellationDelayMaxMs
        {
            get => _cancellationDelayMaxMs;
            set => base.CancellationDelayMaxMs = _cancellationDelayMaxMs = value;
        }

        private string _consumeResultFields = "all";
        public new string ConsumeResultFields
        {
            get => _consumeResultFields;
            set => base.ConsumeResultFields = _consumeResultFields = value;
        }

        #endregion Hidden Inherited Members

        #region IKafkaConsumerConfig Members

        public IEnumerable<string> TopicSubscriptions { get; set; }
        public IEnumerable<TopicPartition> PartitionAssignments { get; set; }
        public bool CommitAfterConsuming { get; set; }
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.Zero;
        public int DefaultBatchSize { get; set; } = 100;
        public bool EnableLogging { get; set; } = true;
        public bool EnableDiagnostics { get; set; } = true;
        public bool EnableDeadLetterTopic { get; set; }
        public bool EnableRetryOnFailure { get; set; }
        public bool EnableInterceptorExceptionPropagation { get; set; }

        #endregion IKafkaConsumerConfig Members

        #region IKafkaConsumerConfigHandler Explicity Members

        void IKafkaConsumerConfigHandler.UpdateTopicSubscriptions(IEnumerable<string> topicSubscriptions)
        {
            TopicSubscriptions = topicSubscriptions;
        }

        void IKafkaConsumerConfigHandler.UpdatePartitionAssignments(IEnumerable<TopicPartition> partitionAssignments)
        {
            PartitionAssignments = partitionAssignments;
        }

        #endregion IKafkaConsumerConfigHandler Explicity Members

        #region Public Methods

        public static IKafkaConsumerConfigBuilder CreateBuilder() => new KafkaConsumerConfigBuilder();

        #endregion Public Methods

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IKafkaConsumerConfig consumerConfig = validationContext?.ObjectInstance as KafkaConsumerConfig ?? this;

            if (string.IsNullOrWhiteSpace(consumerConfig.BootstrapServers))
            {
                yield return new ValidationResult(
                    $"{nameof(consumerConfig.BootstrapServers)} cannot be null or whitespace.",
                    [nameof(consumerConfig.BootstrapServers)]);
            }

            if (consumerConfig.HasTopicSubscriptions() && consumerConfig.HasPartitionAssignments())
            {
                yield return new ValidationResult(
                    $"Both {nameof(consumerConfig.TopicSubscriptions)} and {nameof(consumerConfig.PartitionAssignments)} have been set up.",
                    [nameof(consumerConfig.TopicSubscriptions), nameof(consumerConfig.PartitionAssignments)]);
            }

            if (string.IsNullOrWhiteSpace(consumerConfig.GroupId))
            {
                yield return new ValidationResult(
                    $"{nameof(consumerConfig.GroupId)} cannot be null or whitespace.",
                    [nameof(consumerConfig.GroupId)]);
            }

            if (consumerConfig.DefaultTimeout == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(consumerConfig.DefaultTimeout)} cannot be infinite.",
                    [nameof(consumerConfig.DefaultTimeout)]);
            }

            if (validationContext?.Items is not null)
            {
                if (consumerConfig.EnableDeadLetterTopic &&
                    validationContext.Items.TryGetValue(KafkaProducerConstants.DeadLetterProducer, out object deadLetterProducer) &&
                    deadLetterProducer is null)
                {
                    yield return new ValidationResult(
                        $"{KafkaProducerConstants.DeadLetterProducer} cannot be null when {nameof(consumerConfig.EnableDeadLetterTopic)} is enabled.",
                        [KafkaProducerConstants.DeadLetterProducer, nameof(consumerConfig.EnableDeadLetterTopic)]);
                }

                if (consumerConfig.EnableRetryOnFailure &&
                    validationContext.Items.TryGetValue(KafkaRetryConstants.RetryHandler, out object retryHandler) &&
                    retryHandler is null)
                {
                    yield return new ValidationResult(
                        $"{KafkaRetryConstants.RetryHandler} cannot be null when {nameof(consumerConfig.EnableRetryOnFailure)} is enabled.",
                        [KafkaRetryConstants.RetryHandler, nameof(consumerConfig.EnableRetryOnFailure)]);
                }
            }
        }

        #endregion IValidatableObject Members
    }
}
