﻿using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerOptions<TKey, TValue> : IKafkaConsumerOptions<TKey, TValue>
    {
        public Type ConsumerType { get; init; }
        public ILoggerFactory LoggerFactory { get; init; }
        public IKafkaConsumerConfig ConsumerConfig { get; init; }
        public IKafkaDiagnosticsManager DiagnosticsManager { get; init; }
        public IDeserializer<TKey> KeyDeserializer { get; init; }
        public IDeserializer<TValue> ValueDeserializer { get; init; }
        public Func<TValue, object> MessageIdHandler { get; init; }
        public IRetryHandler<TKey, TValue> RetryHandler { get; init; }
        public IKafkaProducer<byte[], KafkaMetadataMessage> DeadLetterProducer { get; init; }
        public IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> Interceptors { get; init; }
        public Action<IClient, string> OAuthBearerTokenRefreshHandler { get; init; }
        public ICollection<Action<IConsumer<TKey, TValue>, string>> StatisticsHandlers { get; init; }
        public ICollection<Action<IConsumer<TKey, TValue>, Error>> ErrorHandlers { get; init; }
        public ICollection<Action<IConsumer<TKey, TValue>, LogMessage>> LogHandlers { get; init; }
        public ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartition>>> PartitionsAssignedHandlers { get; init; }
        public ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>>> PartitionsRevokedHandlers { get; init; }
        public ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>>> PartitionsLostHandlers { get; init; }
        public ICollection<Action<IConsumer<TKey, TValue>, CommittedOffsets>> OffsetsCommittedHandlers { get; init; }
    }
}
