using Confluent.Kafka.Core.Internal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class AsyncLinkedSemaphore : IAsyncSemaphore
    {
        private static readonly Type DefaultLinkedSemaphoreType = typeof(AsyncLinkedSemaphore);

        private readonly IReadOnlyList<IAsyncSemaphore> _semaphores;

        private AsyncLinkedSemaphore(IReadOnlyList<IAsyncSemaphore> semaphores)
        {
            _semaphores = semaphores ?? throw new ArgumentNullException(nameof(semaphores));
        }

        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            var semaphoreTasks = new List<Task>();

            for (var i = 0; i < _semaphores.Count; i++)
            {
                var semaphore = _semaphores[i];

                semaphoreTasks.Add(semaphore.WaitAsync(cancellationToken));
            }

            await Task.WhenAll(semaphoreTasks).ConfigureAwait(false);
        }

        public void Release()
        {
            CheckDisposed();

            for (var i = _semaphores.Count - 1; i >= 0; i--)
            {
                var semaphore = _semaphores[i];

                semaphore.Release();
            }
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(DefaultLinkedSemaphoreType.ExtractTypeName());
        }

        public static IAsyncSemaphore CreateLinkedSemaphore(params IAsyncSemaphore[] semaphores)
            => new AsyncLinkedSemaphore(semaphores);

        #region IDisposable Members

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_semaphores is not null && _semaphores.Count > 0)
                    {
                        for (var i = 0; i < _semaphores.Count; i++)
                        {
                            var semaphore = _semaphores[i];

                            semaphore?.Dispose();
                        }
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
