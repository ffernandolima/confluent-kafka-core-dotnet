using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal sealed class KafkaConsumerWorkerOptions<TKey, TValue> : IKafkaConsumerWorkerOptions<TKey, TValue>
    {
        public Type WorkerType { get; init; }
        public ILoggerFactory LoggerFactory { get; init; }
        public IKafkaConsumerWorkerConfig WorkerConfig { get; init; }
        public IDiagnosticsManager DiagnosticsManager { get; init; }
        public IHostApplicationLifetime HostApplicationLifetime { get; init; }
        public IKafkaConsumer<TKey, TValue> Consumer { get; init; }
        public IRetryHandler<TKey, TValue> RetryHandler { get; init; }
        public IIdempotencyHandler<TKey, TValue> IdempotencyHandler { get; init; }
        public IKafkaProducer<byte[], KafkaMetadataMessage> RetryProducer { get; init; }
        public IKafkaProducer<byte[], KafkaMetadataMessage> DeadLetterProducer { get; init; }
    }
}
