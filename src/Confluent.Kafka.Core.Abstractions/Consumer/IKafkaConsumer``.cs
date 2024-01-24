using Confluent.Kafka.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumer<TKey, TValue> : IConsumer<TKey, TValue>
    {
        IKafkaConsumerOptions<TKey, TValue> Options { get; }

        ConsumeResult<TKey, TValue> Consume();

        IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch();

        IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(int batchSize);

        IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(TimeSpan timeout);

        IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(int batchSize, TimeSpan timeout);

        IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(CancellationToken cancellationToken);

        IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(int batchSize, CancellationToken cancellationToken);

        IEnumerable<TopicPartitionOffset> SeekBatch(IEnumerable<ConsumeResult<TKey, TValue>> results);

        IEnumerable<KafkaTopicPartitionLag> Lag();
    }
}
