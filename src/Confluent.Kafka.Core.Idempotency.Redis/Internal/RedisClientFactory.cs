using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisClientFactory : IRedisClientFactory
    {
        private static readonly Lazy<RedisClientFactory> Factory = new(
            () => new RedisClientFactory(), isThreadSafe: true);

        public static RedisClientFactory Instance => Factory.Value;

        private RedisClientFactory()
        { }

        public IConnectionMultiplexer GetOrCreateRedisClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IConfigurationOptionsBuilder> configureOptions,
            object clientKey)
        {
            var redisClient = serviceProvider?.GetKeyedService<IConnectionMultiplexer>(
                clientKey ?? RedisClientConstants.StackExchangeRedisClientKey) ??
                CreateRedisClient(
                    serviceProvider,
                    configuration,
                    loggerFactory,
                    (_, builder) => configureOptions?.Invoke(builder));

            return redisClient;
        }

        public IConnectionMultiplexer CreateRedisClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IConfigurationOptionsBuilder> configureOptions)
        {
            var options = ConfigurationOptionsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                (serviceProvider, builder) =>
                {
                    configureOptions?.Invoke(serviceProvider, builder);
                    builder.WithLoggerFactory(loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>());
                });

            var redisClient = ConnectionMultiplexer.Connect(options);

            ConnectionMultiplexer.SetFeatureFlag(
                RedisClientConstants.PreventThreadTheftFeatureFlag,
                enabled: true);

            return redisClient;
        }
    }
}
