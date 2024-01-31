﻿using Confluent.Kafka.Core.Diagnostics;
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

        IDiagnosticsManager DiagnosticsManager { get; }

        ISerializer<TKey> KeySerializer { get; }

        ISerializer<TValue> ValueSerializer { get; }

        Func<TValue, object> MessageIdHandler { get; }

        IRetryHandler<TKey, TValue> RetryHandler { get; }

        IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> Interceptors { get; }
    }
}