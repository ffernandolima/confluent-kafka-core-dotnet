using Confluent.Kafka.Core.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class AsyncSemaphore : IAsyncSemaphore
    {
        private static readonly Type DefaultSemaphoreType = typeof(AsyncSemaphore);

        private readonly SemaphoreSlim _semaphore;

        public int CurrentCount => _semaphore.CurrentCount;

        public AsyncSemaphore(int initialCount)
            : this(new SemaphoreSlim(initialCount))
        { }

        public AsyncSemaphore(int initialCount, int maxCount)
            : this(new SemaphoreSlim(initialCount, maxCount))
        { }

        public AsyncSemaphore(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            var waitTask = _semaphore.WaitAsync(cancellationToken);

            return waitTask;
        }

        public void Release()
        {
            CheckDisposed();

            _semaphore.Release();
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(DefaultSemaphoreType.ExtractTypeName());
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
