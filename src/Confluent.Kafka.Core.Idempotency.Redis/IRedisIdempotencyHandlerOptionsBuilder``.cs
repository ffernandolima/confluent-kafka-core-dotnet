using System;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public interface IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>
    {
        IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithGroupId(string groupId);

        IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithConsumerName(string consumerName);

        IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithExpirationInterval(TimeSpan expirationInterval);

        IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, string> messageIdHandler);

        IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithEnableLogging(bool enableLogging);
    }
}
