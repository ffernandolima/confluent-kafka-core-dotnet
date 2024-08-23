using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Conversion.Internal;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Hosting.Retry.Internal;
using Confluent.Kafka.Core.Idempotency;
using Confluent.Kafka.Core.Idempotency.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry;
using Confluent.Kafka.Core.Retry.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public sealed class KafkaRetryConsumerWorkerBuilder :
        IKafkaRetryConsumerWorkerBuilder,
        IOptionsConverter<IKafkaRetryConsumerWorkerOptions>
    {
        #region Private Fields

        private static readonly Type DefaultConsumerWorkerType = typeof(KafkaRetryConsumerWorker);

        private object _workerKey;
        private bool _workerConfigured;
        private IKafkaDiagnosticsManager _diagnosticsManager;
        private IKafkaConsumer<byte[], KafkaMetadataMessage> _consumer;
        private IRetryHandler<byte[], KafkaMetadataMessage> _retryHandler;
        private IIdempotencyHandler<byte[], KafkaMetadataMessage> _idempotencyHandler;
        private IKafkaProducer<byte[], KafkaMetadataMessage> _deadLetterProducer;
        private IKafkaProducer<byte[], byte[]> _sourceProducer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaRetryConsumerWorker _builtWorker;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaRetryConsumerWorkerOptions _builtOptions;

        #endregion Private Fields

        #region Ctors

        public KafkaRetryConsumerWorkerBuilder()
            : this(workerConfig: null)
        { }

        public KafkaRetryConsumerWorkerBuilder(IKafkaRetryConsumerWorkerConfig workerConfig)
        {
            WorkerConfig = workerConfig ?? KafkaRetryConsumerWorkerConfigBuilder.BuildConfig();
        }

        #endregion Ctors

        #region IOptionsConverter Explicity Members

        IKafkaRetryConsumerWorkerOptions IOptionsConverter<IKafkaRetryConsumerWorkerOptions>.ToOptions()
        {
            _builtOptions ??= new KafkaRetryConsumerWorkerOptions
            {
                WorkerType = DefaultConsumerWorkerType,
                LoggerFactory = LoggerFactory,
                WorkerConfig = WorkerConfig,
                DiagnosticsManager = _diagnosticsManager,
                Consumer = _consumer,
                RetryHandler = _retryHandler,
                IdempotencyHandler = _idempotencyHandler,
                SourceProducer = _sourceProducer,
                DeadLetterProducer = _deadLetterProducer,
            };

            return _builtOptions;
        }

        #endregion IOptionsConverter Explicity Members

        #region IKafkaRetryConsumerWorkerBuilder Members

        public IConfiguration Configuration { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IKafkaRetryConsumerWorkerConfig WorkerConfig { get; private set; }

        public IKafkaRetryConsumerWorkerBuilder WithWorkerKey(object workerKey)
        {
            if (_workerKey is not null)
            {
                throw new InvalidOperationException("Worker key may not be specified more than once.");
            }

            _workerKey = workerKey;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithConfiguration(IConfiguration configuration)
        {
            if (Configuration is not null)
            {
                throw new InvalidOperationException("Configuration may not be specified more than once.");
            }

            Configuration = configuration;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            if (LoggerFactory is not null)
            {
                throw new InvalidOperationException("Logger factory may not be specified more than once.");
            }

            LoggerFactory = loggerFactory;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider is not null)
            {
                throw new InvalidOperationException("Service provider may not be specified more than once.");
            }

            ServiceProvider = serviceProvider;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithConsumer(IKafkaConsumer<byte[], KafkaMetadataMessage> consumer)
        {
            if (_consumer is not null)
            {
                throw new InvalidOperationException("Consumer may not be specified more than once.");
            }

            _consumer = consumer;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithRetryHandler(IRetryHandler<byte[], KafkaMetadataMessage> retryHandler)
        {
            if (_retryHandler is not null)
            {
                throw new InvalidOperationException("Retry handler may not be specified more than once.");
            }

            _retryHandler = retryHandler;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithIdempotencyHandler(IIdempotencyHandler<byte[], KafkaMetadataMessage> idempotencyHandler)
        {
            if (_idempotencyHandler is not null)
            {
                throw new InvalidOperationException("Idempotency handler may not be specified more than once.");
            }

            _idempotencyHandler = idempotencyHandler;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithSourceProducer(IKafkaProducer<byte[], byte[]> sourceProducer)
        {
            if (_sourceProducer is not null)
            {
                throw new InvalidOperationException("Source producer may not be specified more than once.");
            }

            _sourceProducer = sourceProducer;
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer)
        {
            if (_deadLetterProducer is not null)
            {
                throw new InvalidOperationException("Dead letter producer may not be specified more than once.");
            }

            _deadLetterProducer = deadLetterProducer;
            _deadLetterProducer?.ValidateAndThrow(KafkaProducerConstants.DeadLetterTopicSuffix);
            return this;
        }

        public IKafkaRetryConsumerWorkerBuilder WithWorkerConfiguration(Action<IKafkaRetryConsumerWorkerConfigBuilder> configureWorker)
        {
            if (_workerConfigured)
            {
                throw new InvalidOperationException("Worker may not be configured more than once.");
            }

            WorkerConfig = KafkaRetryConsumerWorkerConfigBuilder.BuildConfig(Configuration, WorkerConfig, configureWorker);

            _workerConfigured = true;

            return this;
        }

        public IKafkaRetryConsumerWorker Build()
        {
            if (_builtWorker is not null)
            {
                return _builtWorker;
            }

            WorkerConfig.ValidateAndThrow<KafkaRetryConsumerWorkerConfigException>(
                new ValidationContext(WorkerConfig, new Dictionary<object, object>
                {
                    [KafkaIdempotencyConstants.IdempotencyHandler] = _idempotencyHandler,
                    [KafkaProducerConstants.DeadLetterProducer] = _deadLetterProducer,
                    [KafkaRetryConstants.RetryHandler] = _retryHandler
                }
#if NET8_0_OR_GREATER
                .ToFrozenDictionary()));
#else
                ));
#endif

            if (_consumer is null)
            {
                throw new InvalidOperationException("Consumer cannot be null.");
            }

            if (_sourceProducer is null)
            {
                throw new InvalidOperationException("Source Producer cannot be null.");
            }

            _consumer.Options!.ConsumerConfig.ValidateAndThrow<KafkaConsumerConfigException>(
                new ValidationContext(_consumer.Options!.ConsumerConfig, new Dictionary<object, object>
                {
                    [KafkaSenderConstants.Sender] = new KafkaSender(this, KafkaSenderType.Hosting)
                }
#if NET8_0_OR_GREATER
                .ToFrozenDictionary()));
#else
                ));
#endif
            LoggerFactory ??= ServiceProvider?.GetService<ILoggerFactory>();

            _diagnosticsManager = KafkaDiagnosticsManagerFactory.Instance.GetOrCreateDiagnosticsManager(
                ServiceProvider,
                WorkerConfig.EnableDiagnostics,
                configureOptions: null);

            _builtWorker = (IKafkaRetryConsumerWorker)Activator.CreateInstance(DefaultConsumerWorkerType, this);

            return _builtWorker;
        }

        #endregion IKafkaRetryConsumerWorkerBuilder Members

        #region Internal Methods

        internal static IKafkaRetryConsumerWorker Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaRetryConsumerWorkerBuilder> configureWorker,
            object workerKey)
        {
            var builder = Configure(serviceProvider, configuration, loggerFactory, configureWorker, workerKey);

            var worker = builder.Build();

            return worker;
        }

        internal static IKafkaRetryConsumerWorkerBuilder Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaRetryConsumerWorkerBuilder> configureWorker,
            object workerKey)
        {
            var builder = new KafkaRetryConsumerWorkerBuilder()
                .WithWorkerKey(workerKey)
                .WithConfiguration(configuration)
                .WithLoggerFactory(loggerFactory)
                .WithServiceProvider(serviceProvider);

            configureWorker?.Invoke(serviceProvider, builder);

            return builder;
        }

        #endregion Internal Methods

        #region Public Methods

        public static IKafkaRetryConsumerWorkerBuilder CreateBuilder(IKafkaRetryConsumerWorkerConfig workerConfig = null)
            => new KafkaRetryConsumerWorkerBuilder(workerConfig);

        #endregion Public Methods
    }
}
