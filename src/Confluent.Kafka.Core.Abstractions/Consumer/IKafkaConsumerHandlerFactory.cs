using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerHandlerFactory<TKey, TValue>
    {
        Action<IConsumer<TKey, TValue>, string> CreateStatisticsHandler();

        Action<IConsumer<TKey, TValue>, Error> CreateErrorHandler();

        Action<IConsumer<TKey, TValue>, LogMessage> CreateLogHandler();

        Action<IConsumer<TKey, TValue>, List<TopicPartition>> CreatePartitionsAssignedHandler();

        Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> CreatePartitionsRevokedHandler();

        Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> CreatePartitionsLostHandler();

        Func<IKafkaConsumer<TKey, TValue>, object> CreateConsumerIdHandler();

        Func<TValue, object> CreateMessageIdHandler();
    }
}
