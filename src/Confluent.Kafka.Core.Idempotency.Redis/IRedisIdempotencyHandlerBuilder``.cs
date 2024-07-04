using System;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public interface IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithRedisClient(
            Action<IConfigurationOptionsBuilder> configureOptions,
            object clientKey = null);

        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithHandlerOptions(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions);
    }
}
