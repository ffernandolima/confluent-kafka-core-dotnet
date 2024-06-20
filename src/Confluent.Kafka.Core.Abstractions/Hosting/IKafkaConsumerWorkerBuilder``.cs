using Confluent.Kafka.Core.Consumer;
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
    public interface IKafkaConsumerWorkerBuilder<TKey, TValue>
    {
        ILoggerFactory LoggerFactory { get; }

        IServiceProvider ServiceProvider { get; }

        IKafkaConsumerWorkerConfig WorkerConfig { get; }

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithWorkerKey(object workerKey);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithHostApplicationLifetime(IHostApplicationLifetime hostApplicationLifetime);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumer(IKafkaConsumer<TKey, TValue> consumer);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithIdempotencyHandler(IIdempotencyHandler<TKey, TValue> idempotencyHandler);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithRetryProducer(IKafkaProducer<byte[], KafkaMetadataMessage> retryProducer);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumeResultHandler(IConsumeResultHandler<TKey, TValue> consumeResultHandler);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumeResultHandlers(IEnumerable<IConsumeResultHandler<TKey, TValue>> consumeResultHandlers);

        IKafkaConsumerWorkerBuilder<TKey, TValue> WithWorkerConfiguration(Action<IKafkaConsumerWorkerConfigBuilder> configureWorker);
    }
}
