using Confluent.Kafka.Core.Diagnostics;
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

        public IDiagnosticsManager DiagnosticsManager { get; init; }

        public IDeserializer<TKey> KeyDeserializer { get; init; }

        public IDeserializer<TValue> ValueDeserializer { get; init; }

        public Func<TValue, object> MessageIdHandler { get; init; }

        public IRetryHandler<TKey, TValue> RetryHandler { get; init; }

        public Func<IKafkaConsumer<TKey, TValue>, object> ConsumerIdHandler { get; init; }

        public IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> Interceptors { get; init; }
    }
}
