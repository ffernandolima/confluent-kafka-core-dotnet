using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal static class RedisIdempotencyHandlerFactory
    {
        public static IIdempotencyHandler<TKey, TValue> CreateIdempotencyHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            Action<IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler)
        {
            if (configureHandler is null)
            {
                throw new ArgumentNullException(nameof(configureHandler), $"{nameof(configureHandler)} cannot be null.");
            }

            var builder = RedisIdempotencyHandlerBuilder<TKey, TValue>.Configure(configureHandler);

            var redisOptions = builder.RedisOptions;

            if (redisOptions is null)
            {
                throw new InvalidOperationException($"{nameof(redisOptions)} cannot be null.");
            }

            var handlerOptions = builder.HandlerOptions;

            if (handlerOptions is null)
            {
                throw new InvalidOperationException($"{nameof(handlerOptions)} cannot be null.");
            }

            var loggerFactory = serviceProvider?.GetService<ILoggerFactory>();

            var multiplexer = ConnectionMultiplexer.Connect(redisOptions);

            var idempotencyHandler = new RedisIdempotencyHandler<TKey, TValue>(
                loggerFactory,
                multiplexer,
                handlerOptions);

            return idempotencyHandler;
        }
    }
}
