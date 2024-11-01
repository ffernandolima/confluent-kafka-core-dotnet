﻿using Confluent.Kafka.Core.Conversion.Internal;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>, IProducerAccessor<TKey, TValue>
    {
        private readonly ILogger _logger;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IProducer<TKey, TValue> _producer;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IKafkaProducerOptions<TKey, TValue> _options;

        public Handle Handle
        {
            get
            {
                CheckDisposed();
                return _producer.Handle;
            }
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                return _producer.Name;
            }
        }

        public IKafkaProducerOptions<TKey, TValue> Options
        {
            get
            {
                CheckDisposed();
                return _options;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IProducer<TKey, TValue> IProducerAccessor<TKey, TValue>.UnderlyingProducer
        {
            get
            {
                CheckDisposed();
                return _producer;
            }
        }

        public KafkaProducer(IKafkaProducerBuilder<TKey, TValue> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var options = builder.ToOptions<IKafkaProducerOptions<TKey, TValue>>();

            _logger = options.LoggerFactory.CreateLogger(options.ProducerConfig!.EnableLogging, options.ProducerType);
            _producer = builder.BuildUnderlyingProducer();
            _options = options;
        }

        public int AddBrokers(string brokers)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(brokers))
            {
                throw new ArgumentException($"{nameof(brokers)} cannot be null or whitespace.", nameof(brokers));
            }

            var brokersResult = _producer.AddBrokers(brokers);

            return brokersResult;
        }

        public void SetSaslCredentials(string username, string password)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException($"{nameof(username)} cannot be null or whitespace.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"{nameof(password)} cannot be null or whitespace.", nameof(password));
            }

            _producer.SetSaslCredentials(username, password);
        }

        public void Produce(
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            Produce(_options.ProducerConfig!.DefaultTopic, _options.ProducerConfig!.DefaultPartition, message, deliveryHandler);
        }

        public void Produce(
            string topic,
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            Produce(topic, _options.ProducerConfig!.DefaultPartition, message, deliveryHandler);
        }

        public void Produce(
            Partition partition,
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            Produce(_options.ProducerConfig!.DefaultTopic, partition, message, deliveryHandler);
        }

        public void Produce(
            string topic,
            Partition partition,
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            Produce(new TopicPartition(topic, partition), message, deliveryHandler);
        }

        public void Produce(
            TopicPartition topicPartition,
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            CheckDisposed();

            topicPartition.ValidateAndThrow();

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!_options.ProducerConfig!.EnableRetryOnFailure)
            {
                ProduceInternal(topicPartition, message, deliveryHandler);
            }
            else
            {
                _options.RetryHandler!.Handle(
                    executeAction: _ => ProduceInternal(topicPartition, message, deliveryHandler),
                    onRetryAction: (exception, _, retryAttempt) => OnProduceRetry(topicPartition, message, exception, retryAttempt));
            }
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            Message<TKey, TValue> message,
            CancellationToken cancellationToken = default)
        {
            var deliveryResult = await ProduceAsync(_options.ProducerConfig!.DefaultTopic, _options.ProducerConfig!.DefaultPartition, message, cancellationToken)
                .ConfigureAwait(false);

            return deliveryResult;
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            string topic,
            Message<TKey, TValue> message,
            CancellationToken cancellationToken = default)
        {
            var deliveryResult = await ProduceAsync(topic, _options.ProducerConfig!.DefaultPartition, message, cancellationToken)
                .ConfigureAwait(false);

            return deliveryResult;
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
           Partition partition,
           Message<TKey, TValue> message,
           CancellationToken cancellationToken = default)
        {
            var deliveryResult = await ProduceAsync(_options.ProducerConfig!.DefaultTopic, partition, message, cancellationToken)
                .ConfigureAwait(false);

            return deliveryResult;
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            string topic,
            Partition partition,
            Message<TKey, TValue> message,
            CancellationToken cancellationToken = default)
        {
            var deliveryResult = await ProduceAsync(new TopicPartition(topic, partition), message, cancellationToken)
                .ConfigureAwait(false);

            return deliveryResult;
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            TopicPartition topicPartition,
            Message<TKey, TValue> message,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            topicPartition.ValidateAndThrow();

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            DeliveryResult<TKey, TValue> deliveryResult = null;

            if (!_options.ProducerConfig!.EnableRetryOnFailure)
            {
                deliveryResult = await ProduceInternalAsync(topicPartition, message, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await _options.RetryHandler!.HandleAsync(
                    executeAction: async cancellationToken =>
                        deliveryResult = await ProduceInternalAsync(topicPartition, message, cancellationToken)
                            .ConfigureAwait(false),
                    cancellationToken: cancellationToken,
                    onRetryAction: (exception, _, retryAttempt) => OnProduceRetry(topicPartition, message, exception, retryAttempt))
                .ConfigureAwait(false);
            }

            return deliveryResult;
        }

        public int Poll(TimeSpan timeout)
        {
            CheckDisposed();

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var pollResult = _producer.Poll(timeout);

            return pollResult;
        }

        public int Flush(TimeSpan timeout)
        {
            CheckDisposed();

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var flushResult = _producer.Flush(timeout);

            return flushResult;
        }

        public void Flush(CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            _producer.Flush(cancellationToken);
        }

        public void InitTransactions(TimeSpan timeout)
        {
            CheckDisposed();

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            _producer.InitTransactions(timeout);
        }

        public void BeginTransaction()
        {
            CheckDisposed();

            _producer.BeginTransaction();
        }

        public void CommitTransaction(TimeSpan timeout)
        {
            CheckDisposed();

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            _producer.CommitTransaction(timeout);
        }

        public void CommitTransaction()
        {
            CheckDisposed();

            _producer.CommitTransaction();
        }

        public void AbortTransaction(TimeSpan timeout)
        {
            CheckDisposed();

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            _producer.AbortTransaction(timeout);
        }

        public void AbortTransaction()
        {
            CheckDisposed();

            _producer.AbortTransaction();
        }

        public void SendOffsetsToTransaction(
            IEnumerable<TopicPartitionOffset> offsets,
            IConsumerGroupMetadata groupMetadata,
            TimeSpan timeout)
        {
            CheckDisposed();

            if (offsets is null || offsets.All(offset => offset is null))
            {
                throw new ArgumentException($"{nameof(offsets)} cannot be null, empty, or contain null values.", nameof(offsets));
            }

            if (groupMetadata is null)
            {
                throw new ArgumentNullException(nameof(groupMetadata));
            }

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var transactionOffsets = offsets.Where(offset => offset is not null).Distinct();

            _producer.SendOffsetsToTransaction(transactionOffsets, groupMetadata, timeout);
        }

        private void ProduceInternal(
            TopicPartition topicPartition,
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler)
        {
            Intercept(topicPartition, message);

            message.EnsureDefaultMetadata();

            var messageId = message.GetId(_options.MessageIdHandler);

            var produceResult = new ProduceResult<TKey, TValue>(topicPartition, message);

            using var activity = StartActivity(topicPartition.Topic, message.Headers!.ToDictionary());

            _logger.LogProducingNewMessage(messageId, topicPartition.Topic, topicPartition.Partition);

            var enableDeliveryReports = _options.ProducerConfig!.EnableDeliveryReports.GetValueOrDefault(defaultValue: true);

            try
            {
                if (enableDeliveryReports)
                {
                    _producer.Produce(topicPartition, message, deliveryReport =>
                    {
                        _logger.LogCallbackEventsServed(messageId);

                        produceResult.Complete(deliveryReport);

                        deliveryHandler?.Invoke(deliveryReport);
                    });

                    if (_options.ProducerConfig!.PollAfterProducing)
                    {
                        try
                        {
                            _producer.Poll(_options.ProducerConfig!.DefaultTimeout);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogPollFailure(ex);
                        }
                    }
                }
                else
                {
                    _producer.Produce(topicPartition, message);
                }

                if (!produceResult.DeliveryHandled)
                {
                    if (enableDeliveryReports)
                    {
                        _logger.LogCallbackEventsNotServed(messageId);
                    }
                    else
                    {
                        _logger.LogDeliveryReportsDisabled(messageId);
                    }

                    activity?.SetStatus(ActivityStatusCode.Ok);
                }
                else if (!produceResult.Faulted)
                {
                    _logger.LogMessageProductionSuccess(
                        messageId,
                        produceResult.DeliveryReport!.Topic,
                        produceResult.DeliveryReport!.Partition,
                        produceResult.DeliveryReport!.Offset);

                    activity?.SetStatus(ActivityStatusCode.Ok);
                }
                else
                {
                    _logger.LogMessageProductionFailure(
                        messageId,
                        produceResult.DeliveryReport!.Topic,
                        produceResult.DeliveryReport!.Partition,
                        produceResult.DeliveryReport!.Error);

                    activity?.SetStatus(ActivityStatusCode.Error);
                }

                _options.DiagnosticsManager!.Enrich(activity, produceResult.DeliveryReport, _options);
            }
            catch (ProduceException<TKey, TValue> ex)
            {
                HandleProduceException(ex, activity, messageId);

                throw;
            }
        }

        private async Task<DeliveryResult<TKey, TValue>> ProduceInternalAsync(
            TopicPartition topicPartition,
            Message<TKey, TValue> message,
            CancellationToken cancellationToken)
        {
            Intercept(topicPartition, message);

            message.EnsureDefaultMetadata();

            var messageId = message.GetId(_options.MessageIdHandler);

            using var activity = StartActivity(topicPartition.Topic, message.Headers!.ToDictionary());

            _logger.LogProducingNewMessage(messageId, topicPartition.Topic, topicPartition.Partition);

            try
            {
                var deliveryResult = await _producer.ProduceAsync(topicPartition, message, cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogMessageProductionSuccess(
                    messageId,
                    deliveryResult.Topic,
                    deliveryResult.Partition,
                    deliveryResult.Offset);

                activity?.SetStatus(ActivityStatusCode.Ok);

                _options.DiagnosticsManager!.Enrich(activity, deliveryResult, _options);

                return deliveryResult;
            }
            catch (ProduceException<TKey, TValue> ex)
            {
                HandleProduceException(ex, activity, messageId);

                throw;
            }
        }

        private void Intercept(TopicPartition topicPartition, Message<TKey, TValue> message)
        {
            if (!_options.Interceptors!.Any())
            {
                return;
            }

            var context = new KafkaProducerInterceptorContext<TKey, TValue>
            {
                Message = message,
                TopicPartition = topicPartition,
                ProducerConfig = _options.ProducerConfig
            };

            foreach (var interceptor in _options.Interceptors)
            {
                try
                {
                    interceptor.OnProduce(context);
                }
                catch (Exception ex)
                {
                    var messageId = message.GetId(_options.MessageIdHandler);

                    _logger.LogMessageProductionInterceptionFailure(
                        ex,
                        messageId,
                        topicPartition.Topic,
                        topicPartition.Partition);

                    if (_options.ProducerConfig!.EnableInterceptorExceptionPropagation)
                    {
                        throw;
                    }
                }
            }
        }

        private void OnProduceRetry(TopicPartition topicPartition, Message<TKey, TValue> message, Exception exception, int retryAttempt)
        {
            var messageId = message.GetId(_options.MessageIdHandler);

            switch (exception)
            {
                case ProduceException<TKey, TValue> produceException:
                    {
                        _logger.LogMessageProductionRetryFailure(
                            exception,
                            retryAttempt,
                            messageId,
                            produceException.DeliveryResult!.Topic,
                            produceException.DeliveryResult!.Partition,
                            produceException.Error);
                    }
                    break;
                default:
                    {
                        _logger.LogMessageProductionRetryFailure(
                            exception,
                            retryAttempt,
                            messageId,
                            topicPartition.Topic,
                            topicPartition.Partition);
                    }
                    break;
            }
        }

        private void HandleProduceException(ProduceException<TKey, TValue> produceException, Activity activity, object messageId)
        {
            _logger.LogMessageProductionFailure(
                produceException,
                messageId,
                produceException.DeliveryResult!.Topic,
                produceException.DeliveryResult!.Partition,
                produceException.Error);

            activity?.SetStatus(ActivityStatusCode.Error);

            _options.DiagnosticsManager!.Enrich(activity, produceException, _options);
        }

        private Activity StartActivity(string topic, IDictionary<string, string> headers)
        {
            var activityName = $"{topic} {OperationNames.PublishOperation}";

            var activity = _options.DiagnosticsManager!.StartProducerActivity(activityName, headers);

            return activity;
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(_options.ProducerType!.ExtractTypeName());
        }

        #region IDisposable Members

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _producer?.Flush(_options.ProducerConfig!.DefaultTimeout);
                    _producer?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
