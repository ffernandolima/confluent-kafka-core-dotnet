using Confluent.Kafka.Core.Idempotency.Redis;
using Confluent.Kafka.Core.Idempotency.Redis.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisIdempotencyHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisIdempotencyHandler<TKey, TValue>(
            this IServiceCollection services,
            Action<IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.TryAddSingleton(serviceProvider =>
            {
                var idempotencyHandler = RedisIdempotencyHandlerFactory.CreateIdempotencyHandler(
                    serviceProvider,
                    configureHandler);

                return idempotencyHandler;
            });

            return services;
        }
    }
}
