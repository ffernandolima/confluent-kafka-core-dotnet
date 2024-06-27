﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandlerFactory
    {
        private static readonly Lazy<RedisIdempotencyHandlerFactory> Factory = new(
           () => new RedisIdempotencyHandlerFactory(), isThreadSafe: true);

        public static RedisIdempotencyHandlerFactory Instance => Factory.Value;

        private RedisIdempotencyHandlerFactory()
        { }

        public IIdempotencyHandler<TKey, TValue> GetOrCreateIdempotencyHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler,
            object handlerKey)
        {
            var idempotencyHandler = serviceProvider?.GetKeyedService<IIdempotencyHandler<TKey, TValue>>(
                handlerKey ?? RedisIdempotencyHandlerConstants.RedisIdempotencyHandlerKey) ??
                CreateIdempotencyHandler<TKey, TValue>(
                    serviceProvider,
                    configuration,
                    loggerFactory,
                    (_, builder) => configureHandler?.Invoke(builder));

            return idempotencyHandler;
        }

        public IIdempotencyHandler<TKey, TValue> CreateIdempotencyHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler)
        {
            var builder = RedisIdempotencyHandlerBuilder<TKey, TValue>.Configure(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureHandler);

            var multiplexer = ConnectionMultiplexer.Connect(builder.RedisOptions);

            var idempotencyHandler = new RedisIdempotencyHandler<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                multiplexer,
                builder.HandlerOptions);

            return idempotencyHandler;
        }
    }
}
