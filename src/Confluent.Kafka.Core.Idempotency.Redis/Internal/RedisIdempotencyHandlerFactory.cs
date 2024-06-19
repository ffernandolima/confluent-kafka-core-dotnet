using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal static class RedisIdempotencyHandlerFactory
    {
        public static IIdempotencyHandler<TKey, TValue> GetOrCreateIdempotencyHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            Action<IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler,
            object handlerKey)
        {
            var idempotencyHandler = serviceProvider?.GetKeyedService<IIdempotencyHandler<TKey, TValue>>(
                handlerKey ?? RedisIdempotencyHandlerConstants.RedisIdempotencyHandlerKey) ??
                CreateIdempotencyHandler<TKey, TValue>(serviceProvider, loggerFactory, (_, builder) => configureHandler?.Invoke(builder));

            return idempotencyHandler;
        }

        public static IIdempotencyHandler<TKey, TValue> CreateIdempotencyHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler)
        {
            var builder = RedisIdempotencyHandlerBuilder<TKey, TValue>.Configure(serviceProvider, configureHandler);

            var multiplexer = ConnectionMultiplexer.Connect(builder.RedisOptions);

            var idempotencyHandler = new RedisIdempotencyHandler<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                multiplexer,
                builder.HandlerOptions);

            return idempotencyHandler;
        }
    }
}
