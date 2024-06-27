using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Idempotency.Redis.Internal;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis
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
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
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
