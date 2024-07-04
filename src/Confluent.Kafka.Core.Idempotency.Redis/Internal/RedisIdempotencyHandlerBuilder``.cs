using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandlerBuilder<TKey, TValue> : IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public IConnectionMultiplexer RedisClient { get; private set; }
        public RedisIdempotencyHandlerOptions<TKey, TValue> HandlerOptions { get; private set; }

        public RedisIdempotencyHandlerBuilder(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithRedisClient(
            Action<IConfigurationOptionsBuilder> configureOptions,
            object clientKey = null)
        {
            RedisClient = RedisClientFactory.Instance.GetOrCreateRedisClient(
                _serviceProvider,
                _configuration,
                _loggerFactory,
                configureOptions,
                clientKey);

            return this;
        }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithHandlerOptions(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions)
        {
            HandlerOptions = RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>.Build(
                _configuration,
                configureOptions);

            return this;
        }

        public static RedisIdempotencyHandlerBuilder<TKey, TValue> Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler)
        {
            var builder = new RedisIdempotencyHandlerBuilder<TKey, TValue>(
                serviceProvider,
                configuration,
                loggerFactory);

            configureHandler?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
