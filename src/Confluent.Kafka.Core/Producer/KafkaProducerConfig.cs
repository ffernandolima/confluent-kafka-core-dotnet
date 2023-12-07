using Confluent.Kafka.Core.Producer.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Confluent.Kafka.Core.Producer
{
    public class KafkaProducerConfig : ProducerConfig, IKafkaProducerConfig
    {
        #region Ctors

        public KafkaProducerConfig()
            : base()
        { }

        public KafkaProducerConfig(ProducerConfig config)
            : base(config)
        { }

        public KafkaProducerConfig(ClientConfig config)
            : base(config)
        { }

        public KafkaProducerConfig(IDictionary<string, string> config)
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

        #endregion Hidden Inherited Members

        #region IKafkaProducerConfig Members

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.Zero;
        public bool EnableLogging { get; set; } = true;
        public bool EnableDiagnostics { get; set; } = true;
        public bool EnableRetryOnFailure { get; set; }
        public bool EnableInterceptorExceptionPropagation { get; set; }

        #endregion IKafkaProducerConfig Members

        #region Public Methods

        public static IKafkaProducerConfigBuilder CreateBuilder() => new KafkaProducerConfigBuilder();

        #endregion Public Methods

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IKafkaProducerConfig producerConfig = validationContext?.ObjectInstance as KafkaProducerConfig ?? this;

            if (string.IsNullOrWhiteSpace(producerConfig.BootstrapServers))
            {
                yield return new ValidationResult(
                    $"{nameof(producerConfig.BootstrapServers)} cannot be null or whitespace.",
                    new[] { nameof(producerConfig.BootstrapServers) });
            }

            if (producerConfig.DefaultTimeout == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(producerConfig.DefaultTimeout)} cannot be infinite.",
                    new[] { nameof(producerConfig.DefaultTimeout) });
            }

            if (validationContext?.Items is not null)
            {
                const string RetryHandler = "RetryHandler";

                if (producerConfig.EnableRetryOnFailure && validationContext.Items.TryGetValue(RetryHandler, out object retryHandler) && retryHandler is null)
                {
                    yield return new ValidationResult(
                        $"{RetryHandler} cannot be null when {nameof(producerConfig.EnableRetryOnFailure)} is enabled.",
                        new[] { RetryHandler, nameof(producerConfig.EnableRetryOnFailure) });
                }
            }
        }

        #endregion IValidatableObject Members
    }
}
