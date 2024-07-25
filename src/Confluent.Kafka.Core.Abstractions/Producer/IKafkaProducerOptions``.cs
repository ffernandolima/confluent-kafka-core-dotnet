using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerOptions<TKey, TValue>
    {
        Type ProducerType { get; }

        ILoggerFactory LoggerFactory { get; }

        IKafkaProducerConfig ProducerConfig { get; }

        IKafkaDiagnosticsManager DiagnosticsManager { get; }

        ISerializer<TKey> KeySerializer { get; }

        ISerializer<TValue> ValueSerializer { get; }

        Func<TValue, object> MessageIdHandler { get; }

        IRetryHandler<TKey, TValue> RetryHandler { get; }

        IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> Interceptors { get; }

        Action<IClient, string> OAuthBearerTokenRefreshHandler { get; }

        ICollection<Action<IProducer<TKey, TValue>, string>> StatisticsHandlers { get; }

        ICollection<Action<IProducer<TKey, TValue>, Error>> ErrorHandlers { get; }

        ICollection<Action<IProducer<TKey, TValue>, LogMessage>> LogHandlers { get; }
    }
}
