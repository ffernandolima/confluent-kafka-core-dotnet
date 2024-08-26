using Confluent.Kafka.Core.Conversion.Internal;
using Confluent.Kafka.Core.Hosting.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Mapping.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting.Retry.Internal
{
    internal class KafkaRetryConsumerWorker : KafkaConsumerWorker<byte[], KafkaMetadataMessage>, IKafkaRetryConsumerWorker
    {
        private readonly IKafkaRetryConsumerWorkerOptions _options;

        public KafkaRetryConsumerWorker(IKafkaRetryConsumerWorkerBuilder builder)
            : this(
                  builder?.ToOptions<IKafkaRetryConsumerWorkerOptions>())
        { }

        public KafkaRetryConsumerWorker(IKafkaRetryConsumerWorkerOptions options)
            : base(
                  options?.Map<IKafkaConsumerWorkerOptions<byte[], KafkaMetadataMessage>>(
                      new KafkaInnerRetryConsumerWorker(options)))
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task BeforeProcessingAsync(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult, CancellationToken cancellationToken)
        {
            var messageConsumptionTimeDiff = DateTime.UtcNow - consumeResult.Message.Timestamp.UtcDateTime;

            if (messageConsumptionTimeDiff < _options.WorkerConfig.RetryTopicDelay)
            {
                var retryDelayDiff = _options.WorkerConfig.RetryTopicDelay - messageConsumptionTimeDiff;

                Logger.LogDelayingRetryUntil(DateTime.UtcNow.Add(retryDelayDiff));

                try
                {
                    await Task.Delay(retryDelayDiff, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        protected override bool ShouldBypassIdempotency(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult) => false;

        private sealed class KafkaInnerRetryConsumerWorker :
            IConsumeResultHandler<byte[], KafkaMetadataMessage>,
            IKafkaConsumerLifecycleWorker<byte[], KafkaMetadataMessage>
        {
            private readonly ILogger _logger;
            private readonly IKafkaRetryConsumerWorkerOptions _options;

            public KafkaInnerRetryConsumerWorker(IKafkaRetryConsumerWorkerOptions options)
            {
                if (options is null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                _logger = options.LoggerFactory.CreateLogger(options.WorkerConfig!.EnableLogging, options.WorkerType);
                _options = options;
            }

            public Task StartAsync(IKafkaConsumerWorkerOptions<byte[], KafkaMetadataMessage> options, CancellationToken cancellationToken)
            {
                var producerConfig = _options.SourceProducer!.Options!.ProducerConfig;

                if (options.Consumer!.Subscription!.Count == 0 &&
                    options.Consumer!.Assignment!.Count == 0 &&
                    !string.IsNullOrWhiteSpace(producerConfig!.DefaultTopic))
                {
                    options.Consumer!.Subscribe([$"{producerConfig!.DefaultTopic}{KafkaRetryConstants.RetryTopicSuffix}"]);
                }

                return Task.CompletedTask;
            }

            public Task StopAsync(IKafkaConsumerWorkerOptions<byte[], KafkaMetadataMessage> options, CancellationToken cancellationToken) => Task.CompletedTask;

            public async Task HandleAsync(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult, CancellationToken cancellationToken)
            {
                if (consumeResult?.Message is null)
                {
                    _logger.LogNonValidMessageReceived();
                    return;
                }

                await HandleInternalAsync(consumeResult, cancellationToken).ConfigureAwait(false);
            }

            private async Task HandleInternalAsync(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult, CancellationToken cancellationToken)
            {
                var retryCount = GetRetryCount(consumeResult);

                if (++retryCount <= _options.WorkerConfig!.RetryCount)
                {
                    UpdateRetryCount(consumeResult, retryCount);

                    await ProduceSourceMessageAsync(consumeResult, retryCount, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var sourceTopic = GetSourceTopic(consumeResult);

                    _logger.LogMaximumRetryAttemptsReached(_options.WorkerConfig!.RetryCount, consumeResult.Message!.Value!.SourceId, sourceTopic);

                    if (_options.WorkerConfig!.EnableDeadLetterTopic)
                    {
                        await ProduceDeadLetterMessageAsync(consumeResult, _options.WorkerConfig!.RetryCount, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.LogDeadLetterTopicStrategyDisabled(sourceTopic, consumeResult.Message!.Value!.SourceId);
                    }
                }
            }

            private int GetRetryCount(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult)
            {
                var retryCount = 0;

                var headers = consumeResult.Message!.Headers?.ToDictionary();

                if (headers is not null && headers.TryGetValue(KafkaRetryConstants.RetryCountKey, out string headerValue))
                {
                    if (!int.TryParse(headerValue, out retryCount))
                    {
                        _logger.LogRetryHeaderUnexpectedType(KafkaRetryConstants.RetryCountKey, headerValue, consumeResult.Message!.Value!.SourceId);
                    }
                }
                else
                {
                    _logger.LogRetryHeaderNotFound(KafkaRetryConstants.RetryCountKey, consumeResult.Message!.Value!.SourceId);
                }

                return retryCount;
            }

            private void UpdateRetryCount(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult, int retryCount)
            {
                var headers = consumeResult.Message!.Headers?.ToDictionary();

                headers?.AddOrUpdate(KafkaRetryConstants.RetryCountKey, retryCount.ToString());
            }

            private string GetSourceTopic(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult)
            {
                string sourceTopic;

                var messageValue = consumeResult.Message!.Value;

                var sourceConfig = _options.SourceProducer!.Options!.ProducerConfig;

                if (!string.IsNullOrWhiteSpace(sourceConfig!.DefaultTopic))
                {
                    if (sourceConfig!.DefaultTopic != messageValue!.SourceTopic)
                    {
                        _logger.LogUnmatchedSourceTopic(sourceConfig!.DefaultTopic, messageValue!.SourceTopic, messageValue!.SourceId);
                    }

                    sourceTopic = sourceConfig!.DefaultTopic;
                }
                else
                {
                    sourceTopic = messageValue!.SourceTopic;
                }

                return sourceTopic;
            }

            private async Task ProduceSourceMessageAsync(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult, int retryCount, CancellationToken cancellationToken)
            {
                var sourceTopic = GetSourceTopic(consumeResult);

                _logger.LogProducingSourceMessage(consumeResult.Message!.Value!.SourceId, sourceTopic, retryCount);

                await _options.SourceProducer!.ProduceAsync(
                    sourceTopic,
                    new Message<byte[], byte[]>
                    {
                        Key = consumeResult.Message!.Value!.SourceKey,
                        Value = consumeResult.Message!.Value!.SourceValue,
                        Headers = consumeResult.Message!.Headers
                    },
                    cancellationToken)
                    .ConfigureAwait(false);
            }

            private async Task ProduceDeadLetterMessageAsync(ConsumeResult<byte[], KafkaMetadataMessage> consumeResult, int retryCount, CancellationToken cancellationToken)
            {
                var producerConfig = _options.DeadLetterProducer!.Options!.ProducerConfig;

                var deadLetterTopic = !string.IsNullOrWhiteSpace(producerConfig!.DefaultTopic)
                    ? producerConfig!.DefaultTopic
                    : $"{consumeResult.Topic}{KafkaProducerConstants.DeadLetterTopicSuffix}";

                _logger.LogProducingDeadLetterMessage(consumeResult.Message!.Value!.SourceId, deadLetterTopic, retryCount);

                await _options.DeadLetterProducer!.ProduceAsync(
                    deadLetterTopic,
                    new Message<byte[], KafkaMetadataMessage>
                    {
                        Key = consumeResult.Message!.Key,
                        Value = consumeResult.Message!.Value!.Clone(),
                        Headers = consumeResult.Message!.Headers
                    },
                    cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        #region IDisposable Members

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _options.SourceProducer?.Dispose();
                }

                _disposed = true;

                base.Dispose(disposing);
            }
        }

        #endregion IDisposable Members
    }
}
