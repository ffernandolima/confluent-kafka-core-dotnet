using Confluent.Kafka.Core.Consumer.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Confluent.Kafka.Core.Consumer
{
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue> // TODO: Make this internal sealed?
    {
        private readonly ILogger _logger;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IConsumer<TKey, TValue> _consumer;
        private readonly IKafkaConsumerOptions<TKey, TValue> _options;

        private object _id;
        public object Id => _id ??= _options.ConsumerIdHandler?.Invoke(this);

        public Handle Handle => _consumer.Handle;
        public string Name => _consumer.Name;
        public string MemberId => _consumer.MemberId;
        public List<string> Subscription => _consumer.Subscription;
        public List<TopicPartition> Assignment => _consumer.Assignment;
        public IConsumerGroupMetadata ConsumerGroupMetadata => _consumer.ConsumerGroupMetadata;

        public KafkaConsumer(IKafkaConsumerBuilder<TKey, TValue> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            _logger = builder.CreateLogger();
            _consumer = builder.BuildInnerConsumer();
            _options = builder.ToOptions();
        }

        public int AddBrokers(string brokers)
            => _consumer.AddBrokers(brokers);

        public void Assign(TopicPartition partition)
            => _consumer.Assign(partition);

        public void Assign(TopicPartitionOffset partition)
            => _consumer.Assign(partition);

        public void Assign(IEnumerable<TopicPartitionOffset> partitions)
            => _consumer.Assign(partitions);

        public void Assign(IEnumerable<TopicPartition> partitions)
            => _consumer.Assign(partitions);

        public void Close()
            => _consumer.Close();

        public List<TopicPartitionOffset> Commit()
            => _consumer.Commit();

        public void Commit(IEnumerable<TopicPartitionOffset> offsets)
            => _consumer.Commit(offsets);

        public void Commit(ConsumeResult<TKey, TValue> consumeResult)
            => _consumer.Commit(consumeResult);

        public List<TopicPartitionOffset> Committed(TimeSpan timeout)
            => _consumer.Committed(timeout);

        public List<TopicPartitionOffset> Committed(IEnumerable<TopicPartition> partitions, TimeSpan timeout)
            => _consumer.Committed(partitions, timeout);

        public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
            => _consumer.Consume(millisecondsTimeout);

        public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
            => _consumer.Consume(cancellationToken);

        public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
            => _consumer.Consume(timeout);

        public IEnumerable<ConsumeResult<TKey, TValue>> Consume(int batchSize, int millisecondsTimeout)
        {
            var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

            var results = Consume(batchSize, timeout);

            return results;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> Consume(int batchSize, TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var source = new CancellationTokenSource(timeout);

            var results = Consume(batchSize, source.Token);

            return results;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> Consume(int batchSize, CancellationToken cancellationToken)
        {
            List<ConsumeResult<TKey, TValue>> consumeResults = null;

            if (batchSize <= 0)
            {
                throw new ArgumentException($"{nameof(batchSize)} cannot be less than or equal to zero.", $"{nameof(batchSize)}");
            }

            var cancellationDelay = TimeSpan.FromMilliseconds(_options.ConsumerConfig!.CancellationDelayMaxMs);

            while ((consumeResults?.Count ?? 0) < batchSize && !cancellationToken.IsCancellationRequested)
            {
                var consumeResult = Consume(cancellationDelay);

                if (consumeResult is not null)
                {
                    consumeResults ??= new List<ConsumeResult<TKey, TValue>>();
                    consumeResults.Add(consumeResult);
                }
            }

            return consumeResults?.AsReadOnly();
        }

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition partition)
            => _consumer.GetWatermarkOffsets(partition);

        public void IncrementalAssign(IEnumerable<TopicPartitionOffset> partitions)
            => _consumer.IncrementalAssign(partitions);

        public void IncrementalAssign(IEnumerable<TopicPartition> partitions)
            => _consumer.IncrementalAssign(partitions);

        public void IncrementalUnassign(IEnumerable<TopicPartition> partitions)
            => _consumer.IncrementalUnassign(partitions);

        public List<TopicPartitionOffset> OffsetsForTimes(IEnumerable<TopicPartitionTimestamp> timestampsToSearch, TimeSpan timeout)
            => _consumer.OffsetsForTimes(timestampsToSearch, timeout);

        public void Pause(IEnumerable<TopicPartition> partitions)
            => _consumer.Pause(partitions);

        public Offset Position(TopicPartition partition)
            => _consumer.Position(partition);

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition partition, TimeSpan timeout)
            => _consumer.QueryWatermarkOffsets(partition, timeout);

        public void Resume(IEnumerable<TopicPartition> partitions)
            => _consumer.Resume(partitions);

        public void Seek(TopicPartitionOffset offset)
            => _consumer.Seek(offset);

        public IEnumerable<TopicPartitionOffset> Seek(IEnumerable<ConsumeResult<TKey, TValue>> consumeResults)
        {
            if (consumeResults is null || !consumeResults.Any())
            {
                throw new ArgumentException($"{nameof(consumeResults)} cannot be null or empty.", nameof(consumeResults));
            }

            var offsets = consumeResults.Where(result => result is not null)
                                        .GroupBy(result => result.TopicPartition)
                                        .Select(sublist => new TopicPartitionOffset(sublist.Key, new Offset(sublist.Min(x => x.Offset))));

            foreach (var offset in offsets)
            {
                _logger.LogSeekingTopicPartitionOffset(offset.Topic, offset.Partition, offset.Offset);

                _consumer.Seek(offset);
            }

            return offsets;
        }

        public void SetSaslCredentials(string username, string password)
            => _consumer.SetSaslCredentials(username, password);

        public void StoreOffset(ConsumeResult<TKey, TValue> consumeResult)
            => _consumer.StoreOffset(consumeResult);

        public void StoreOffset(TopicPartitionOffset offset)
            => _consumer.StoreOffset(offset);

        public void Subscribe(IEnumerable<string> topics)
            => _consumer.Subscribe(topics);

        public void Subscribe(string topic)
            => _consumer.Subscribe(topic);

        public void Unassign()
            => _consumer.Unassign();

        public void Unsubscribe()
            => _consumer.Unsubscribe();

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _consumer?.Close();
                    _consumer?.Dispose();
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
