using Confluent.Kafka.Core.Hosting.Retry;
using Confluent.Kafka.Core.Idempotency.Redis.Internal;
using Confluent.Kafka.Core.Models;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public static class RedisIdempotencyHandlerKafkaRetryConsumerWorkerBuilderExtensions
    {
        public static IKafkaRetryConsumerWorkerBuilder WithRedisIdempotencyHandler(
            this IKafkaRetryConsumerWorkerBuilder builder,
            Action<IRedisIdempotencyHandlerBuilder<byte[], KafkaMetadataMessage>> configureHandler = null,
            object handlerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var idempotencyHandler = RedisIdempotencyHandlerFactory.Instance.GetOrCreateIdempotencyHandler(
                builder.ServiceProvider,
                builder.Configuration,
                builder.LoggerFactory,
                configureHandler,
                handlerKey);

            builder.WithIdempotencyHandler(idempotencyHandler);

            return builder;
        }
    }
}
