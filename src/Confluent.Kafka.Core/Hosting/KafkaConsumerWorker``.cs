using Confluent.Kafka.Core.Hosting.Internal;
using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerWorker<TKey, TValue> : BackgroundService, IKafkaConsumerWorker<TKey, TValue>
    {
        private readonly ILogger _logger;
        private readonly IKafkaConsumerWorkerOptions<TKey, TValue> _options;

        public IKafkaConsumerWorkerOptions<TKey, TValue> Options
        {
            get
            {
                CheckDisposed();
                return _options;
            }
        }

        public KafkaConsumerWorker(IKafkaConsumerWorkerBuilder<TKey, TValue> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var options = builder.ToOptions();

            _logger = options.LoggerFactory.CreateLogger(options.WorkerConfig!.EnableLogging, options.WorkerType);
            _options = options;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(_options.WorkerType!.ExtractTypeName());
        }

        #region IDisposable Members

        private bool _disposed;

        public override void Dispose()
        {
            _disposed = true;

            base.Dispose();
        }

        #endregion IDisposable Members
    }
}
