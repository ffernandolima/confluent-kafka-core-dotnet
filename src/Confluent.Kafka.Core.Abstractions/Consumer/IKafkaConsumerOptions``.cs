using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerOptions<TKey, TValue>
    {
        Type ConsumerType { get; }

        ILoggerFactory LoggerFactory { get; }

        IKafkaConsumerConfig ConsumerConfig { get; }

        IKafkaDiagnosticsManager DiagnosticsManager { get; }

        IDeserializer<TKey> KeyDeserializer { get; }

        IDeserializer<TValue> ValueDeserializer { get; }

        Func<TValue, object> MessageIdHandler { get; }

        IRetryHandler<TKey, TValue> RetryHandler { get; }

        IKafkaProducer<byte[], KafkaMetadataMessage> DeadLetterProducer { get; }

        IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> Interceptors { get; }

        Action<IClient, string> OAuthBearerTokenRefreshHandler { get; }

        ICollection<Action<IConsumer<TKey, TValue>, string>> StatisticsHandlers { get; }

        ICollection<Action<IConsumer<TKey, TValue>, Error>> ErrorHandlers { get; }

        ICollection<Action<IConsumer<TKey, TValue>, LogMessage>> LogHandlers { get; }

        ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartition>>> PartitionsAssignedHandlers { get; }

        ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>>> PartitionsRevokedHandlers { get; }

        ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>>> PartitionsLostHandlers { get; }

        ICollection<Action<IConsumer<TKey, TValue>, CommittedOffsets>> OffsetsCommittedHandlers { get; }

    }
}
