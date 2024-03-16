using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandlerOptionsBuilder<TKey, TValue> :
        FunctionalBuilder<RedisIdempotencyHandlerOptions<TKey, TValue>, RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>>,
        IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>
    {
        public IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithGroupId(string groupId)
        {
            AppendAction(options => options.GroupId = groupId);
            return this;
        }

        public IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithConsumerName(string consumerName)
        {
            AppendAction(options => options.ConsumerName = consumerName);
            return this;
        }

        public IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithExpirationInterval(TimeSpan expirationInterval)
        {
            AppendAction(options => options.ExpirationInterval = expirationInterval);
            return this;
        }

        public IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, string> messageIdHandler)
        {
            AppendAction(options => options.MessageIdHandler = messageIdHandler);
            return this;
        }

        public IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithEnableLogging(bool enableLogging)
        {
            AppendAction(options => options.EnableLogging = enableLogging);
            return this;
        }

        public static RedisIdempotencyHandlerOptions<TKey, TValue> Build(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions)
        {
            using var builder = new RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>();

            configureOptions?.Invoke(builder);

            var options = builder.Build();

            return options;
        }
    }
}
