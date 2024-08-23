using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public interface IKafkaRetryConsumerWorkerOptions
    {
        Type WorkerType { get; }

        ILoggerFactory LoggerFactory { get; }

        IKafkaRetryConsumerWorkerConfig WorkerConfig { get; }

        IKafkaDiagnosticsManager DiagnosticsManager { get; }

        IKafkaConsumer<byte[], KafkaMetadataMessage> Consumer { get; }

        IRetryHandler<byte[], KafkaMetadataMessage> RetryHandler { get; }

        IIdempotencyHandler<byte[], KafkaMetadataMessage> IdempotencyHandler { get; }

        IKafkaProducer<byte[], byte[]> SourceProducer { get; }

        IKafkaProducer<byte[], KafkaMetadataMessage> DeadLetterProducer { get; }
    }
}
