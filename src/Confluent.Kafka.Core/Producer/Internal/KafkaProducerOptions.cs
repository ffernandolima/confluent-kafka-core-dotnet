using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerOptions<TKey, TValue> : IKafkaProducerOptions<TKey, TValue>
    {
        public Type ProducerType { get; init; }

        public ILoggerFactory LoggerFactory { get; init; }

        public IKafkaProducerConfig ProducerConfig { get; init; }

        public IDiagnosticsManager DiagnosticsManager { get; init; }

        public ISerializer<TKey> KeySerializer { get; init; }

        public ISerializer<TValue> ValueSerializer { get; init; }

        public Func<TValue, object> MessageIdHandler { get; init; }

        public IRetryHandler<TKey, TValue> RetryHandler { get; init; }

        public Func<IKafkaProducer<TKey, TValue>, object> ProducerIdHandler { get; init; }

        public IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> Interceptors { get; init; }
    }
}
