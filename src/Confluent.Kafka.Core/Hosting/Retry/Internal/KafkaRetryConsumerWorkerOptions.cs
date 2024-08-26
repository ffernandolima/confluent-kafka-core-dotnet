using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Hosting.Internal;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Mapping.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace Confluent.Kafka.Core.Hosting.Retry.Internal
{
    internal sealed class KafkaRetryConsumerWorkerOptions :
        IKafkaRetryConsumerWorkerOptions,
        IMapper<IKafkaConsumerWorkerOptions<byte[], KafkaMetadataMessage>>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumerWorkerOptions<byte[], KafkaMetadataMessage> _mappedOptions;

        public Type WorkerType { get; init; }
        public ILoggerFactory LoggerFactory { get; init; }
        public IKafkaRetryConsumerWorkerConfig WorkerConfig { get; init; }
        public IKafkaDiagnosticsManager DiagnosticsManager { get; init; }
        public IKafkaConsumer<byte[], KafkaMetadataMessage> Consumer { get; init; }
        public IRetryHandler<byte[], KafkaMetadataMessage> RetryHandler { get; init; }
        public IIdempotencyHandler<byte[], KafkaMetadataMessage> IdempotencyHandler { get; init; }
        public IKafkaProducer<byte[], byte[]> SourceProducer { get; init; }
        public IKafkaProducer<byte[], KafkaMetadataMessage> DeadLetterProducer { get; init; }

        #region IMapper Members

        public IKafkaConsumerWorkerOptions<byte[], KafkaMetadataMessage> Map(params object[] args)
        {
            _mappedOptions ??= new KafkaConsumerWorkerOptions<byte[], KafkaMetadataMessage>
            {
                WorkerType = WorkerType,
                LoggerFactory = LoggerFactory,
                WorkerConfig = WorkerConfig.Map<IKafkaConsumerWorkerConfig>(),
                DiagnosticsManager = DiagnosticsManager,
                Consumer = Consumer,
                RetryHandler = RetryHandler,
                IdempotencyHandler = IdempotencyHandler,
                DeadLetterProducer = DeadLetterProducer,
                ConsumeResultHandlers = args?.OfType<IConsumeResultHandler<byte[], KafkaMetadataMessage>>(),
                ConsumerLifecycleWorker = args?.OfType<IKafkaConsumerLifecycleWorker<byte[], KafkaMetadataMessage>>()?.SingleOrDefault()
            };

            return _mappedOptions;
        }

        #endregion IMapper Members
    }
}
