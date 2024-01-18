using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Producer
{
    public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue> // TODO: Make this internal sealed?
    {
        private readonly ILogger _logger;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IProducer<TKey, TValue> _producer;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IKafkaProducerOptions<TKey, TValue> _options;

        public Handle Handle => _producer.Handle;
        public string Name => _producer.Name;
        public IKafkaProducerOptions<TKey, TValue> Options => _options;

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

        public void AbortTransaction(TimeSpan timeout)
            => _producer.AbortTransaction(timeout);

        public void AbortTransaction()
            => _producer.AbortTransaction();

        public int AddBrokers(string brokers)
            => _producer.AddBrokers(brokers);

        public void BeginTransaction()
            => _producer.BeginTransaction();

        public void CommitTransaction(TimeSpan timeout)
            => _producer.CommitTransaction(timeout);

        public void CommitTransaction()
            => _producer.CommitTransaction();

        public int Flush(TimeSpan timeout)
            => _producer.Flush(timeout);

        public void Flush(CancellationToken cancellationToken = default)
            => _producer.Flush(cancellationToken);

        public void InitTransactions(TimeSpan timeout)
            => _producer.InitTransactions(timeout);

        public int Poll(TimeSpan timeout)
            => _producer.Poll(timeout);

        public void Produce(string topic, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
            => _producer.Produce(topic, message, deliveryHandler);

        public void Produce(TopicPartition topicPartition, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
            => _producer.Produce(topicPartition, message, deliveryHandler);

        public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message, CancellationToken cancellationToken = default)
            => _producer.ProduceAsync(topic, message, cancellationToken);

        public Task<DeliveryResult<TKey, TValue>> ProduceAsync(TopicPartition topicPartition, Message<TKey, TValue> message, CancellationToken cancellationToken = default)
            => _producer.ProduceAsync(topicPartition, message, cancellationToken);

        public void SendOffsetsToTransaction(IEnumerable<TopicPartitionOffset> offsets, IConsumerGroupMetadata groupMetadata, TimeSpan timeout)
            => _producer.SendOffsetsToTransaction(offsets, groupMetadata, timeout);

        public void SetSaslCredentials(string username, string password)
            => _producer.SetSaslCredentials(username, password);

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
