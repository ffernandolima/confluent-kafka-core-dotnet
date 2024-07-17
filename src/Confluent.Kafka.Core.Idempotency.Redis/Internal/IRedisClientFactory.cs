using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal interface IRedisClientFactory
    {
        IConnectionMultiplexer GetOrCreateRedisClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IConfigurationOptionsBuilder> configureOptions,
            object clientKey);

        IConnectionMultiplexer CreateRedisClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IConfigurationOptionsBuilder> configureOptions);
    }
}
