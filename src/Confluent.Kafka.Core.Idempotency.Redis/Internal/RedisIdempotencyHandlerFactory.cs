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
            Action<IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler = null)
        {
            var builder = new RedisIdempotencyHandlerBuilder<TKey, TValue>();

            configureHandler?.Invoke(builder);

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
