using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandlerOptionsBuilder<TKey, TValue> :
        FunctionalBuilder<RedisIdempotencyHandlerOptions<TKey, TValue>, RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>>,
        IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>
    {
        public RedisIdempotencyHandlerOptionsBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> FromConfiguration(string sectionKey)
        {
            AppendAction(options =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    options = Bind(options, sectionKey);
                }
            });
            return this;
        }

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

        public IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue> WithExpirationDelay(TimeSpan expirationDelay)
        {
            AppendAction(options => options.ExpirationDelay = expirationDelay);
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
            IConfiguration configuration,
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions)
        {
            using var builder = new RedisIdempotencyHandlerOptionsBuilder<TKey, TValue>(configuration);

            configureOptions?.Invoke(builder);

            var options = builder.Build();

            return options;
        }
    }
}
