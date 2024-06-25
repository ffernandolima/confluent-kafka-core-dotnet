using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandlerBuilder<TKey, TValue> : IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        private readonly IConfiguration _configuration;

        public ConfigurationOptions RedisOptions { get; private set; }
        public RedisIdempotencyHandlerOptions<TKey, TValue> HandlerOptions { get; private set; }

        public RedisIdempotencyHandlerBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithRedis(
            Action<IConfigurationOptionsBuilder> configureOptions)
        {
            RedisOptions = ConfigurationOptionsBuilder.Build(_configuration, configureOptions);
            return this;
        }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithHandlerOptions(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions)
        {
            HandlerOptions = RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>.Build(_configuration, configureOptions);
            return this;
        }

        public static RedisIdempotencyHandlerBuilder<TKey, TValue> Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler)
        {
            var builder = new RedisIdempotencyHandlerBuilder<TKey, TValue>(configuration);

            configureHandler?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
