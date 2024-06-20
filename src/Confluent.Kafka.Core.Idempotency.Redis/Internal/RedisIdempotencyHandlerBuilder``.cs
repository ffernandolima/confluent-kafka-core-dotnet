using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandlerBuilder<TKey, TValue> : IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        public ConfigurationOptions RedisOptions { get; private set; }
        public RedisIdempotencyHandlerOptions<TKey, TValue> HandlerOptions { get; private set; }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithRedisOptions(
            Action<IConfigurationOptionsBuilder> configureOptions)
        {
            RedisOptions = ConfigurationOptionsBuilder.Build(configureOptions);
            return this;
        }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithHandlerOptions(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions)
        {
            HandlerOptions = RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>.Build(configureOptions);
            return this;
        }

        public static RedisIdempotencyHandlerBuilder<TKey, TValue> Configure(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IRedisIdempotencyHandlerBuilder<TKey, TValue>> configureHandler)
        {
            var builder = new RedisIdempotencyHandlerBuilder<TKey, TValue>();

            configureHandler?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
