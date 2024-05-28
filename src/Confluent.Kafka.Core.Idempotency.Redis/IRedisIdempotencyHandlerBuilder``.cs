using System;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public interface IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithRedisConfiguration(
            Action<IConfigurationOptionsBuilder> configureOptions);

        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithHandlerConfiguration(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions);
    }
}
