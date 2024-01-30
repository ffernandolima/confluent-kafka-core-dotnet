using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Producer
{
    public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>, IProducerAccessor<TKey, TValue> // TODO: Make this internal sealed?
    {
        private readonly ILogger _logger;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IProducer<TKey, TValue> _producer;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IKafkaProducerOptions<TKey, TValue> _options;

        public Handle Handle => _producer.Handle;
        public string Name => _producer.Name;
        public IKafkaProducerOptions<TKey, TValue> Options => _options;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IProducer<TKey, TValue> IProducerAccessor<TKey, TValue>.UnderlyingProducer => _producer;

        public KafkaProducer(IKafkaProducerBuilder<TKey, TValue> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var options = builder.ToOptions();

            _logger = options.LoggerFactory.CreateLogger(options.ProducerConfig!.EnableLogging, options.ProducerType);
            _producer = builder.BuildUnderlyingProducer();
            _options = options;
        }

        public int AddBrokers(string brokers)
        {
            if (string.IsNullOrWhiteSpace(brokers))
            {
                throw new ArgumentException($"{nameof(brokers)} cannot be null or whitespace.", nameof(brokers));
            }

            var brokersResult = _producer.AddBrokers(brokers);

            return brokersResult;
        }

        public void SetSaslCredentials(string username, string password)
        {
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

        public void Produce(string topic, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
            => _producer.Produce(topic, message, deliveryHandler);

        public void Produce(TopicPartition topicPartition, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
            => _producer.Produce(topicPartition, message, deliveryHandler);

        public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message, CancellationToken cancellationToken = default)
            => _producer.ProduceAsync(topic, message, cancellationToken);

        public Task<DeliveryResult<TKey, TValue>> ProduceAsync(TopicPartition topicPartition, Message<TKey, TValue> message, CancellationToken cancellationToken = default)
            => _producer.ProduceAsync(topicPartition, message, cancellationToken);

        public int Poll(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var pollResult = _producer.Poll(timeout);

            return pollResult;
        }

        public int Flush(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var flushResult = _producer.Flush(timeout);

            return flushResult;
        }

        public void Flush(CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                throw new ArgumentException($"{nameof(cancellationToken)} should be capable of being canceled.", nameof(cancellationToken));
            }

            _producer.Flush(cancellationToken);
        }

        public void InitTransactions(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            _producer.InitTransactions(timeout);
        }

        public void BeginTransaction()
        {
            _producer.BeginTransaction();
        }

        public void CommitTransaction(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            _producer.CommitTransaction(timeout);
        }

        public void CommitTransaction()
        {
            _producer.CommitTransaction();
        }

        public void AbortTransaction(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            _producer.AbortTransaction(timeout);
        }

        public void AbortTransaction()
        {
            _producer.AbortTransaction();
        }

        public void SendOffsetsToTransaction(IEnumerable<TopicPartitionOffset> offsets, IConsumerGroupMetadata groupMetadata, TimeSpan timeout)
        {
            if (offsets is null || !offsets.Any(offset => offset is not null))
            {
                throw new ArgumentException($"{nameof(offsets)} cannot be null, empty, or contain null values.", nameof(offsets));
            }

            if (groupMetadata is null)
            {
                throw new ArgumentNullException(nameof(groupMetadata), $"{nameof(groupMetadata)} cannot be null.");
            }

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var transactionOffsets = offsets.Where(offset => offset is not null).Distinct();

            _producer.SendOffsetsToTransaction(transactionOffsets, groupMetadata, timeout);
        }

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
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
