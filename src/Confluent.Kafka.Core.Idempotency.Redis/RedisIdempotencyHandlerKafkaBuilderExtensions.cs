using Confluent.Kafka.Core.Idempotency.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class RedisIdempotencyHandlerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddRedisIdempotencyHandler<TKey, TValue>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler,
            object handlerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureHandler is null)
            {
                throw new ArgumentNullException(nameof(configureHandler));
            }

            builder.Services!.AddRedisIdempotencyHandler(configureHandler, handlerKey);

            return builder;
        }
    }
}
