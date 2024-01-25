using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Retry.Polly;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class RedisIdempotencyHandler<TKey, TValue> : IIdempotencyHandler<TKey, TValue>
    {
        private static readonly Type DefaultIdempotencyHandlerType = typeof(RedisIdempotencyHandler<TKey, TValue>);
        private static readonly string MessageValueTypeName = typeof(TValue).ExtractTypeName();

        private readonly ILogger _logger;
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly RedisIdempotencyHandlerOptions<TKey, TValue> _options;

        private string Key => $"Idempotency:GroupIds:{_options.GroupId}:Consumers:{_options.ConsumerName}";

        public RedisIdempotencyHandler(
            ILoggerFactory loggerFactory,
            IConnectionMultiplexer multiplexer,
            RedisIdempotencyHandlerOptions<TKey, TValue> options)
        {
            if (multiplexer is null)
            {
                throw new ArgumentNullException(nameof(multiplexer), $"{nameof(multiplexer)} cannot be null.");
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            options.ValidateAndThrow<RedisIdempotencyHandlerOptionsException<TKey, TValue>>();

            _logger = loggerFactory.CreateLogger(options.EnableLogging, DefaultIdempotencyHandlerType);
            _multiplexer = multiplexer;
            _options = options;
        }

        public async Task<bool> TryHandleAsync(TValue messageValue)
        {
            var messageId = _options.MessageIdHandler.Invoke(messageValue);

            if (string.IsNullOrWhiteSpace(messageId))
            {
                _logger.LogMessageIdNotFound(Key, MessageValueTypeName);

                return true;
            }

            try
            {
                await TryRemoveExpiredItemsAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);

                var addedSuccessfully = await TryAddItemWhenNotExistsAsync(messageId)
                    .ConfigureAwait(continueOnCapturedContext: false);

                return addedSuccessfully;
            }
            catch (RedisException ex)
            {
                _logger.LogIdempotencyHandlingFailure(ex, Key);

                return true;
            }
        }

        private async Task TryRemoveExpiredItemsAsync()
        {
            var database = _multiplexer.GetDatabase();

            var maximumScore = ToUnixTimeSeconds(DateTime.UtcNow.Subtract(_options.ExpirationInterval));

            var affected = await database.SortedSetRemoveRangeByScoreAsync(Key, 0, maximumScore)
                .ConfigureAwait(continueOnCapturedContext: false);

            if (affected > 0)
            {
                _logger.LogSortedSetMembersExpired(affected, Key);
            }
        }

        private async Task<bool> TryAddItemWhenNotExistsAsync(string identifier)
        {
            var database = _multiplexer.GetDatabase();

            var score = ToUnixTimeSeconds(DateTime.UtcNow);

            var addedSuccessfully = await database.SortedSetAddAsync(Key, identifier, score, When.NotExists)
                .ConfigureAwait(continueOnCapturedContext: false);

            return addedSuccessfully;
        }

        private static long ToUnixTimeSeconds(DateTime source) => ((DateTimeOffset)source).ToUnixTimeSeconds();
    }
}
