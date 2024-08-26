using Confluent.Kafka.Core.Idempotency.Redis;
using Confluent.Kafka.Core.Idempotency.Redis.Internal;
using System;

namespace Confluent.Kafka.Core.Hosting
{
    public static class RedisIdempotencyHandlerKafkaConsumerWorkerBuilderExtensions
    {
        public static IKafkaConsumerWorkerBuilder<TKey, TValue> WithRedisIdempotencyHandler<TKey, TValue>(
            this IKafkaConsumerWorkerBuilder<TKey, TValue> builder,
            Action<IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler = null,
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
