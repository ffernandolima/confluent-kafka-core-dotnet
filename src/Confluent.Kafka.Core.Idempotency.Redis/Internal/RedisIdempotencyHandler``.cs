using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Retry.Polly;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading;
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

        private Task _expirationTask;
        private CancellationTokenSource _expirationCts;

        private string _key;
        private string Key => _key ??= $"Idempotency:GroupIds:{_options.GroupId}:Consumers:{_options.ConsumerName}";

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

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (_expirationCts is null && _expirationTask is null)
            {
                _expirationCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _expirationTask = StartExpirationTask(_expirationCts.Token);
            }

            return _expirationTask.IsCompleted ? _expirationTask : Task.CompletedTask;
        }

        public async Task<bool> TryHandleAsync(TValue messageValue, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            var messageId = _options.MessageIdHandler.Invoke(messageValue);

            if (string.IsNullOrWhiteSpace(messageId))
            {
                _logger.LogMessageIdNotFound(Key, MessageValueTypeName);

                return true;
            }

            try
            {
                var addedSuccessfully = await TryAddItemWhenNotExistsAsync(messageId, cancellationToken)
                    .ConfigureAwait(false);

                return addedSuccessfully;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogIdempotencyHandlingCanceled(ex, Key);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogIdempotencyHandlingFailure(ex, Key);

                return true;
            }
        }

        private void TryRemoveExpiredItems()
        {
            var database = _multiplexer.GetDatabase();

            var maximumScore = ToUnixTimeSeconds(DateTime.UtcNow.Subtract(_options.ExpirationInterval));

            var affected = database.SortedSetRemoveRangeByScore(Key, 0, maximumScore);

            if (affected > 0)
            {
                _logger.LogSortedSetMembersExpired(affected, Key);
            }
        }

        private async Task<bool> TryAddItemWhenNotExistsAsync(string messageId, CancellationToken cancellationToken)
        {
            var database = _multiplexer.GetDatabase();

            var score = ToUnixTimeSeconds(DateTime.UtcNow);

            var addedSuccessfully = await database.SortedSetAddAsync(Key, messageId, score, When.NotExists)
                .WithCancellation(cancellationToken)
                .ConfigureAwait(false);

            return addedSuccessfully;
        }

        private Task StartExpirationTask(CancellationToken cancellationToken)
            => Task.Factory.StartNew(() =>
            {
                try
                {
                    for (; ; )
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        TryRemoveExpiredItems();

                        Task.Delay(_options.ExpirationDelay).Wait(cancellationToken);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogSortedSetMembersExpirationCanceled(ex, Key);
                }
                catch (Exception ex)
                {
                    _logger.LogSortedSetMembersExpirationFailure(ex, Key);
                }

            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(DefaultIdempotencyHandlerType.ExtractTypeName());
        }

        private static long ToUnixTimeSeconds(DateTime source) => ((DateTimeOffset)source).ToUnixTimeSeconds();

        #region IDisposable Members

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        _expirationCts?.Cancel();

                        _expirationTask?.Wait();
                    }
                    finally
                    {
                        _expirationCts?.Dispose();
                    }

                    _multiplexer?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
