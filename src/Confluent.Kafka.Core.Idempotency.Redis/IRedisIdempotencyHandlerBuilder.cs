using System;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public interface IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithConfigureRedisOptions(
            Action<IConfigurationOptionsBuilder> configureOptions);

        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithConfigureHandlerOptions(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions);
    }
}
