using System;
using System.Collections.Generic;
using System.Threading;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumer<TKey, TValue> : IConsumer<TKey, TValue>
    {
        IKafkaConsumerOptions<TKey, TValue> Options { get; }

        IEnumerable<ConsumeResult<TKey, TValue>> Consume(int batchSize, int millisecondsTimeout);

        IEnumerable<ConsumeResult<TKey, TValue>> Consume(int batchSize, TimeSpan timeout);

        IEnumerable<ConsumeResult<TKey, TValue>> Consume(int batchSize, CancellationToken cancellationToken);

        IEnumerable<TopicPartitionOffset> Seek(IEnumerable<ConsumeResult<TKey, TValue>> results);
    }
}
