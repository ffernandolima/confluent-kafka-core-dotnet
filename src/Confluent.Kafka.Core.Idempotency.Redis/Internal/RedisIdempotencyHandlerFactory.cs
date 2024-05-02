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

            var loggerFactory = serviceProvider?.GetService<ILoggerFactory>();

            var multiplexer = ConnectionMultiplexer.Connect(builder.RedisOptions);

            var idempotencyHandler = new RedisIdempotencyHandler<TKey, TValue>(
                loggerFactory,
                multiplexer,
                builder.HandlerOptions);

            return idempotencyHandler;
        }
    }
}
