using Confluent.Kafka.Core.Idempotency.Redis;
using Confluent.Kafka.Core.Idempotency.Redis.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisClientServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisClient(
            this IServiceCollection services,
            Action<IServiceProvider, IConfigurationOptionsBuilder> configureOptions,
            object clientKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.TryAddKeyedSingleton(
                clientKey ?? RedisClientConstants.StackExchangeRedisClientKey,
                (serviceProvider, _) =>
                {
                    var redisClient = RedisClientFactory.Instance.CreateRedisClient(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        serviceProvider.GetService<ILoggerFactory>(),
                        configureOptions);

                    return redisClient;
                });

            return services;
        }
    }
}
