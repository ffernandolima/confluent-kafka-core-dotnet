using Confluent.Kafka.Core.Idempotency.Redis;
using Confluent.Kafka.Core.Idempotency.Redis.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisIdempotencyHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisIdempotencyHandler<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler,
            object handlerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureHandler is null)
            {
                throw new ArgumentNullException(nameof(configureHandler));
            }

            services.TryAddKeyedSingleton(
                handlerKey ?? RedisIdempotencyHandlerConstants.RedisIdempotencyHandlerKey,
                (serviceProvider, _) =>
                {
                    var idempotencyHandler = RedisIdempotencyHandlerFactory.Instance.CreateIdempotencyHandler(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        serviceProvider.GetService<ILoggerFactory>(),
                        configureHandler);

                    return idempotencyHandler;
                });

            return services;
        }
    }
}
