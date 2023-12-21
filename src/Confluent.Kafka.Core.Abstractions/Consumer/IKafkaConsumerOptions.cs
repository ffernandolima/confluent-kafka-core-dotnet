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

        IDiagnosticsManager DiagnosticsManager { get; }

        IDeserializer<TKey> KeyDeserializer { get; }

        IDeserializer<TValue> ValueDeserializer { get; }

        Func<TValue, object> MessageIdHandler { get; }

        IRetryHandler<TKey, TValue> RetryHandler { get; }

        IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> Interceptors { get; }
    }
}
