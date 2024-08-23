using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public interface IKafkaRetryConsumerWorkerBuilder
    {
        IConfiguration Configuration { get; }

        ILoggerFactory LoggerFactory { get; }

        IServiceProvider ServiceProvider { get; }

        IKafkaRetryConsumerWorkerConfig WorkerConfig { get; }

        IKafkaRetryConsumerWorkerBuilder WithWorkerKey(object workerKey);

        IKafkaRetryConsumerWorkerBuilder WithConfiguration(IConfiguration configuration);

        IKafkaRetryConsumerWorkerBuilder WithLoggerFactory(ILoggerFactory loggerFactory);

        IKafkaRetryConsumerWorkerBuilder WithServiceProvider(IServiceProvider serviceProvider);

        IKafkaRetryConsumerWorkerBuilder WithConsumer(IKafkaConsumer<byte[], KafkaMetadataMessage> consumer);

        IKafkaRetryConsumerWorkerBuilder WithRetryHandler(IRetryHandler<byte[], KafkaMetadataMessage> retryHandler);

        IKafkaRetryConsumerWorkerBuilder WithIdempotencyHandler(IIdempotencyHandler<byte[], KafkaMetadataMessage> idempotencyHandler);

        IKafkaRetryConsumerWorkerBuilder WithSourceProducer(IKafkaProducer<byte[], byte[]> sourceProducer);

        IKafkaRetryConsumerWorkerBuilder WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer);

        IKafkaRetryConsumerWorkerBuilder WithWorkerConfiguration(Action<IKafkaRetryConsumerWorkerConfigBuilder> configureWorker);

        IKafkaRetryConsumerWorker Build();
    }
}
