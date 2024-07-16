using Confluent.Kafka.Core.Internal;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Threading.Internal
{
    /// <remarks>Based on: https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-6-asynclock/</remarks>
    internal sealed class AsyncLock : IDisposable
    {
        private static readonly Type DefaultLockType = typeof(AsyncLock);

        private readonly AsyncLockOptions _options;
        private readonly AsyncSemaphore _semaphore;
        private readonly ConcurrentDictionary<object, IAsyncSemaphore> _semaphores = new();

        public int CurrentCount => _semaphore.CurrentCount;

        public AsyncLock(AsyncLockOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            options.ValidateAndThrow<AsyncLockOptionsException>();

            _semaphore = new AsyncSemaphore(options.MaxDegreeOfParallelism, options.MaxDegreeOfParallelism);
            _options = options;
        }

        public async Task<IDisposable> LockAsync(AsyncLockContext context, CancellationToken cancellationToken)
        {
            CheckDisposed();

            object key;

            if (!_options.HandleLockByKey || (key = _options.LockKeyHandler!.Invoke(context)) is null)
            {
                var releaser = await LockAsync(_semaphore, cancellationToken).ConfigureAwait(false);

                return releaser;
            }
            else
            {
                var semaphore = _semaphores.GetOrAdd(
                    key,
                    _ => AsyncLinkedSemaphore.CreateLinkedSemaphore(
                        _semaphore,
                        new AsyncSemaphore(1, 1)));

                var releaser = await LockAsync(semaphore, cancellationToken).ConfigureAwait(false);

                return releaser;
            }
        }

        private async Task<IDisposable> LockAsync(IAsyncSemaphore semaphore, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            var releaser = new AsyncLockReleaser(semaphore);

            return releaser;
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(DefaultLockType.ExtractTypeName());
        }

        private struct AsyncLockReleaser : IDisposable
        {
            private readonly IAsyncSemaphore _semaphore;

            public AsyncLockReleaser(IAsyncSemaphore semaphore)
            {
                _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore), $"{nameof(semaphore)} cannot be null.");
            }

            #region IDisposable Members

            private bool _disposed;

            public void Dispose()
            {
                if (!_disposed)
                {
                    _semaphore?.Release();

                    _disposed = true;
                }
            }

            #endregion IDisposable Members
        }

        #region IDisposable Members

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _semaphore?.Dispose();

                    if (_semaphores is not null && _semaphores.Count > 0)
                    {
                        foreach (var semaphore in _semaphores.Values)
                        {
                            semaphore?.Dispose();
                        }

                        _semaphores.Clear();
                    }
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
