using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Idempotency.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry;
using Confluent.Kafka.Core.Retry.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal sealed class KafkaConsumerWorkerBuilder<TKey, TValue> :
        IKafkaConsumerWorkerBuilder<TKey, TValue>,
        IKafkaConsumerWorkerOptionsConverter<TKey, TValue>
    {
        #region Private Fields

        private static readonly Type DefaultConsumerWorkerType = typeof(KafkaConsumerWorker<TKey, TValue>);

        private object _workerKey;
        private bool _workerConfigured;
        private IDiagnosticsManager _diagnosticsManager;
        private IHostApplicationLifetime _hostApplicationLifetime;
        private IKafkaConsumer<TKey, TValue> _consumer;
        private IRetryHandler<TKey, TValue> _retryHandler;
        private IIdempotencyHandler<TKey, TValue> _idempotencyHandler;
        private IKafkaProducer<byte[], KafkaMetadataMessage> _retryProducer;
        private IKafkaProducer<byte[], KafkaMetadataMessage> _deadLetterProducer;
        private IEnumerable<IConsumeResultHandler<TKey, TValue>> _consumeResultHandlers;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumerWorker<TKey, TValue> _builtWorker;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumerWorkerOptions<TKey, TValue> _builtOptions;

        #endregion Private Fields

        #region Ctors

        public KafkaConsumerWorkerBuilder()
        {
            WorkerConfig = BuildConfig();
        }

        #endregion Ctors

        #region IKafkaConsumerWorkerOptionsConverter Explicity Members

        IKafkaConsumerWorkerOptions<TKey, TValue> IKafkaConsumerWorkerOptionsConverter<TKey, TValue>.ToOptions()
        {
            _builtOptions ??= new KafkaConsumerWorkerOptions<TKey, TValue>
            {
                WorkerType = DefaultConsumerWorkerType,
                LoggerFactory = LoggerFactory,
                WorkerConfig = WorkerConfig,
                DiagnosticsManager = _diagnosticsManager,
                HostApplicationLifetime = _hostApplicationLifetime,
                Consumer = _consumer,
                RetryHandler = _retryHandler,
                IdempotencyHandler = _idempotencyHandler,
                RetryProducer = _retryProducer,
                DeadLetterProducer = _deadLetterProducer
            };

            return _builtOptions;
        }

        #endregion IKafkaConsumerWorkerOptionsConverter Explicity Members

        #region IKafkaConsumerWorkerBuilder Members

        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IKafkaConsumerWorkerConfig WorkerConfig { get; private set; }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithWorkerKey(object workerKey)
        {
            if (_workerKey is not null)
            {
                throw new InvalidOperationException("Worker key may not be specified more than once.");
            }

            _workerKey = workerKey;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            if (LoggerFactory is not null)
            {
                throw new InvalidOperationException("Logger factory may not be specified more than once.");
            }

            LoggerFactory = loggerFactory;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider is not null)
            {
                throw new InvalidOperationException("Service provider may not be specified more than once.");
            }

            ServiceProvider = serviceProvider;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithHostApplicationLifetime(IHostApplicationLifetime hostApplicationLifetime)
        {
            if (_hostApplicationLifetime is not null)
            {
                throw new InvalidOperationException("host application lifetime may not be specified more than once.");
            }

            _hostApplicationLifetime = hostApplicationLifetime;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumer(IKafkaConsumer<TKey, TValue> consumer)
        {
            if (_consumer is not null)
            {
                throw new InvalidOperationException("Consumer may not be specified more than once.");
            }

            _consumer = consumer;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler)
        {
            if (_retryHandler is not null)
            {
                throw new InvalidOperationException("Retry handler may not be specified more than once.");
            }

            _retryHandler = retryHandler;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithIdempotencyHandler(IIdempotencyHandler<TKey, TValue> idempotencyHandler)
        {
            if (_idempotencyHandler is not null)
            {
                throw new InvalidOperationException("Idempotency handler may not be specified more than once.");
            }

            _idempotencyHandler = idempotencyHandler;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithRetryProducer(IKafkaProducer<byte[], KafkaMetadataMessage> retryProducer)
        {
            if (_retryProducer is not null)
            {
                throw new InvalidOperationException("Retry producer may not be specified more than once.");
            }

            _retryProducer = retryProducer;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer)
        {
            if (_deadLetterProducer is not null)
            {
                throw new InvalidOperationException("Dead letter producer may not be specified more than once.");
            }

            _deadLetterProducer = deadLetterProducer;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumeResultHandler(IConsumeResultHandler<TKey, TValue> consumeResultHandler)
        {
            if (consumeResultHandler is not null)
            {
                _consumeResultHandlers = (_consumeResultHandlers ?? []).Union([consumeResultHandler]);
            }
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumeResultHandlers(IEnumerable<IConsumeResultHandler<TKey, TValue>> consumeResultHandlers)
        {
            if (_consumeResultHandlers is not null)
            {
                throw new InvalidOperationException("Consume result handlers may not be specified more than once.");
            }

            if (consumeResultHandlers is not null && consumeResultHandlers.Any(consumeResultHandler => consumeResultHandler is not null))
            {
                _consumeResultHandlers = consumeResultHandlers.Where(consumeResultHandler => consumeResultHandler is not null);
            }
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithWorkerConfiguration(Action<IKafkaConsumerWorkerConfigBuilder> configureWorker)
        {
            if (_workerConfigured)
            {
                throw new InvalidOperationException("Worker may not be configured more than once.");
            }

            BuildConfig(WorkerConfig, configureWorker);

            _workerConfigured = true;

            return this;
        }

        public IKafkaConsumerWorker<TKey, TValue> Build()
        {
            if (_builtWorker is not null)
            {
                return _builtWorker;
            }

            WorkerConfig.ValidateAndThrow<KafkaConsumerWorkerConfigException>(
                new ValidationContext(WorkerConfig, new Dictionary<object, object>
                {
                    [KafkaIdempotencyConstants.IdempotencyHandler] = _idempotencyHandler,
                    [KafkaProducerConstants.DeadLetterProducer] = _deadLetterProducer,
                    [KafkaProducerConstants.RetryProducer] = _retryProducer,
                    [KafkaRetryConstants.RetryHandler] = _retryHandler
                }));

            if (_consumer is null)
            {
                throw new InvalidOperationException("Consumer cannot be null.");
            }

            if (_consumeResultHandlers is null || !_consumeResultHandlers.Any(consumeResultHandler => consumeResultHandler is not null))
            {
                throw new InvalidOperationException("Consume result handlers cannot be null, empty, or contain null values.");
            }

            LoggerFactory ??= ServiceProvider?.GetService<ILoggerFactory>();

            _hostApplicationLifetime ??= ServiceProvider?.GetService<IHostApplicationLifetime>();

            _diagnosticsManager ??= DiagnosticsManagerFactory.GetDiagnosticsManager(
                ServiceProvider,
                WorkerConfig.EnableDiagnostics);

            _builtWorker = (IKafkaConsumerWorker<TKey, TValue>)Activator.CreateInstance(DefaultConsumerWorkerType, this);

            return _builtWorker;
        }

        #endregion IKafkaConsumerWorkerBuilder Members

        #region Private Methods

        private static IKafkaConsumerWorkerConfig BuildConfig(
           IKafkaConsumerWorkerConfig workerConfig = null,
           Action<IKafkaConsumerWorkerConfigBuilder> configureWorker = null)
        {
            using var builder = new KafkaConsumerWorkerConfigBuilder(workerConfig);

            configureWorker?.Invoke(builder);

            return builder.Build();
        }

        #endregion Private Methods
    }
}
