using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IKafkaConsumerWorkerOptions<TKey, TValue>
    {
        Type WorkerType { get; }

        ILoggerFactory LoggerFactory { get; }

        IKafkaConsumerWorkerConfig WorkerConfig { get; }

        IDiagnosticsManager DiagnosticsManager { get; }

        IHostApplicationLifetime HostApplicationLifetime { get; }

        IKafkaConsumer<TKey, TValue> Consumer { get; }

        IRetryHandler<TKey, TValue> RetryHandler { get; }

        IIdempotencyHandler<TKey, TValue> IdempotencyHandler { get; }

        IKafkaProducer<byte[], KafkaMetadataMessage> RetryProducer { get; }

        IKafkaProducer<byte[], KafkaMetadataMessage> DeadLetterProducer { get; }

        IEnumerable<IConsumeResultHandler<TKey, TValue>> ConsumeResultHandlers { get; }
    }
}
