using Confluent.Kafka.Core.Consumer.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

namespace Confluent.Kafka.Core.Consumer
{
    public class KafkaConsumerConfig : ConsumerConfig, IKafkaConsumerConfig
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

        #region IKafkaConsumerConfig Explicity Members

        bool IKafkaConsumerConfig.HasTopicSubscriptions => TopicSubscriptions is not null && TopicSubscriptions.Any(topic => !string.IsNullOrWhiteSpace(topic));
        bool IKafkaConsumerConfig.HasPartitionAssignments => PartitionAssignments is not null && PartitionAssignments.Any(assignment => assignment is not null);

        #endregion IKafkaConsumerConfig Explicity Members

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
                    new[] { nameof(consumerConfig.BootstrapServers) });
            }

            if (consumerConfig.HasTopicSubscriptions && consumerConfig.HasPartitionAssignments)
            {
                yield return new ValidationResult(
                    $"Both {nameof(consumerConfig.TopicSubscriptions)} and {nameof(consumerConfig.PartitionAssignments)} have been set up.",
                    new[] { nameof(consumerConfig.TopicSubscriptions), nameof(consumerConfig.PartitionAssignments) });
            }

            if (string.IsNullOrWhiteSpace(consumerConfig.GroupId))
            {
                yield return new ValidationResult(
                    $"{nameof(consumerConfig.GroupId)} cannot be null or whitespace.",
                    new[] { nameof(consumerConfig.GroupId) });
            }

            if (consumerConfig.DefaultTimeout == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(consumerConfig.DefaultTimeout)} cannot be infinite.",
                    new[] { nameof(consumerConfig.DefaultTimeout) });
            }

            if (validationContext?.Items is not null)
            {
                const string RetryHandler = "RetryHandler";

                if (consumerConfig.EnableRetryOnFailure && validationContext.Items.TryGetValue(RetryHandler, out object retryHandler) && retryHandler is null)
                {
                    yield return new ValidationResult(
                        $"{RetryHandler} cannot be null when {nameof(consumerConfig.EnableRetryOnFailure)} is enabled.",
                        new[] { RetryHandler, nameof(consumerConfig.EnableRetryOnFailure) });
                }
            }
        }

        #endregion IValidatableObject Members
    }
}
