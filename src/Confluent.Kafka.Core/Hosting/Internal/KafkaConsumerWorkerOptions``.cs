using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Confluent.Kafka.Core.Serialization.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal sealed class KafkaConsumerWorkerOptions<TKey, TValue> : IKafkaConsumerWorkerOptions<TKey, TValue>
    {
        private ISerializer<TKey> _keySerializer;
        private ISerializer<TValue> _valueSerializer;

        public Type WorkerType { get; init; }
        public ILoggerFactory LoggerFactory { get; init; }
        public IKafkaConsumerWorkerConfig WorkerConfig { get; init; }
        public IKafkaDiagnosticsManager DiagnosticsManager { get; init; }
        public IKafkaConsumer<TKey, TValue> Consumer { get; init; }
        public IRetryHandler<TKey, TValue> RetryHandler { get; init; }
        public IIdempotencyHandler<TKey, TValue> IdempotencyHandler { get; init; }
        public IKafkaProducer<byte[], KafkaMetadataMessage> RetryProducer { get; init; }
        public IKafkaProducer<byte[], KafkaMetadataMessage> DeadLetterProducer { get; init; }
        public IKafkaConsumerLifecycleWorker<TKey, TValue> ConsumerLifecycleWorker { get; init; }
        public IEnumerable<IConsumeResultHandler<TKey, TValue>> ConsumeResultHandlers { get; init; }
        public IConsumeResultErrorHandler<TKey, TValue> ConsumeResultErrorHandler { get; init; }
        public Func<ConsumeResult<TKey, TValue>, object> MessageOrderGuaranteeKeyHandler { get; init; }

        public ISerializer<TKey> KeySerializer
        {
            get
            {
                if (_keySerializer is null && (WorkerConfig.EnableRetryTopic || WorkerConfig.EnableDeadLetterTopic))
                {
                    _keySerializer = Consumer.Options!.KeyDeserializer.ToSerializer<TKey>();
                }

                return _keySerializer;
            }
        }

        public ISerializer<TValue> ValueSerializer
        {
            get
            {
                if (_valueSerializer is null && (WorkerConfig.EnableRetryTopic || WorkerConfig.EnableDeadLetterTopic))
                {
                    _valueSerializer = Consumer.Options!.ValueDeserializer.ToSerializer<TValue>();
                }

                return _valueSerializer;
            }
        }
    }
}
