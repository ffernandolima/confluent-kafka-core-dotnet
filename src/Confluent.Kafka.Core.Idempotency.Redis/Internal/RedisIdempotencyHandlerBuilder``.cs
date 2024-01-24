using StackExchange.Redis;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandlerBuilder<TKey, TValue> : IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        public ConfigurationOptions RedisOptions { get; private set; }
        public RedisIdempotencyHandlerOptions<TKey, TValue> HandlerOptions { get; private set; }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithConfigureRedisOptions(
            Action<IConfigurationOptionsBuilder> configureOptions)
        {
            RedisOptions = ConfigurationOptionsBuilder.Build(configureOptions);
            ConnectionMultiplexer.SetFeatureFlag("PreventThreadTheft", enabled: true);
            return this;
        }

        public IRedisIdempotencyHandlerBuilder<TKey, TValue> WithConfigureHandlerOptions(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions)
        {
            HandlerOptions = RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>.Build(configureOptions);
            return this;
        }
    }
}
