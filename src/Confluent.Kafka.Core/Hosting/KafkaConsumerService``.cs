using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerService<TKey, TValue> : BackgroundService
    {
        private static readonly Type DefaultServiceType = typeof(KafkaConsumerService<TKey, TValue>);

        private readonly IKafkaConsumerWorker<TKey, TValue> _consumerWorker;

        public KafkaConsumerService(IKafkaConsumerWorker<TKey, TValue> consumerWorker)
        {
            _consumerWorker = consumerWorker ?? throw new ArgumentNullException(nameof(consumerWorker));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            await _consumerWorker.StartAsync(cancellationToken).ConfigureAwait(false);

            await base.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            await _consumerWorker.StopAsync(cancellationToken).ConfigureAwait(false);

            await base.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumerWorker.ExecuteAsync(stoppingToken).ConfigureAwait(false);
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(DefaultServiceType.ExtractTypeName());
        }

        #region IDisposable Members

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    base.Dispose();

                    _consumerWorker?.Dispose();
                }

                _disposed = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
