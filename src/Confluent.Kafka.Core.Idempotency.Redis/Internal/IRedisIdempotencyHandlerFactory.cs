using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal interface IRedisIdempotencyHandlerFactory
    {
        IIdempotencyHandler<TKey, TValue> GetOrCreateIdempotencyHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler,
            object handlerKey);

        IIdempotencyHandler<TKey, TValue> CreateIdempotencyHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler);
    }
}
