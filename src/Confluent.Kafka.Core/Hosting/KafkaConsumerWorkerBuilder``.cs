using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Conversion.Internal;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
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
using System.Linq;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerWorkerBuilder<TKey, TValue> :
        IKafkaConsumerWorkerBuilder<TKey, TValue>,
        IOptionsConverter<IKafkaConsumerWorkerOptions<TKey, TValue>>
    {
        #region Private Fields

        private static readonly Type DefaultConsumerWorkerType = typeof(KafkaConsumerWorker<TKey, TValue>);

        private object _workerKey;
        private bool _workerConfigured;
        private IKafkaConsumer<TKey, TValue> _consumer;
        private IRetryHandler<TKey, TValue> _retryHandler;
        private IKafkaDiagnosticsManager _diagnosticsManager;
        private IIdempotencyHandler<TKey, TValue> _idempotencyHandler;
        private IKafkaProducer<byte[], KafkaMetadataMessage> _retryProducer;
        private IKafkaProducer<byte[], KafkaMetadataMessage> _deadLetterProducer;
        private IKafkaConsumerLifecycleWorker<TKey, TValue> _consumerLifecycleWorker;
        private IEnumerable<IConsumeResultHandler<TKey, TValue>> _consumeResultHandlers;
        private IConsumeResultErrorHandler<TKey, TValue> _consumeResultErrorHandler;
        private Func<ConsumeResult<TKey, TValue>, object> _messageOrderGuaranteeKeyHandler;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumerWorker<TKey, TValue> _builtWorker;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumerWorkerOptions<TKey, TValue> _builtOptions;

        #endregion Private Fields

        #region Ctors

        public KafkaConsumerWorkerBuilder()
            : this(workerConfig: null)
        { }

        public KafkaConsumerWorkerBuilder(IKafkaConsumerWorkerConfig workerConfig)
        {
            WorkerConfig = workerConfig ?? KafkaConsumerWorkerConfigBuilder.BuildConfig();
        }

        #endregion Ctors

        #region IOptionsConverter Explicity Members

        IKafkaConsumerWorkerOptions<TKey, TValue> IOptionsConverter<IKafkaConsumerWorkerOptions<TKey, TValue>>.ToOptions()
        {
            _builtOptions ??= new KafkaConsumerWorkerOptions<TKey, TValue>
            {
                WorkerType = DefaultConsumerWorkerType,
                LoggerFactory = LoggerFactory,
                WorkerConfig = WorkerConfig,
                DiagnosticsManager = _diagnosticsManager,
                Consumer = _consumer,
                RetryHandler = _retryHandler,
                IdempotencyHandler = _idempotencyHandler,
                RetryProducer = _retryProducer,
                DeadLetterProducer = _deadLetterProducer,
                ConsumerLifecycleWorker = _consumerLifecycleWorker,
                ConsumeResultHandlers = _consumeResultHandlers,
                ConsumeResultErrorHandler = _consumeResultErrorHandler,
                MessageOrderGuaranteeKeyHandler = _messageOrderGuaranteeKeyHandler
            };

            return _builtOptions;
        }

        #endregion IOptionsConverter Explicity Members

        #region IKafkaConsumerWorkerBuilder Members

        public IConfiguration Configuration { get; private set; }
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

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithConfiguration(IConfiguration configuration)
        {
            if (Configuration is not null)
            {
                throw new InvalidOperationException("Configuration may not be specified more than once.");
            }

            Configuration = configuration;
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
            _retryProducer?.ValidateAndThrow(KafkaRetryConstants.RetryTopicSuffix);
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer)
        {
            if (_deadLetterProducer is not null)
            {
                throw new InvalidOperationException("Dead letter producer may not be specified more than once.");
            }

            _deadLetterProducer = deadLetterProducer;
            _deadLetterProducer?.ValidateAndThrow(KafkaProducerConstants.DeadLetterTopicSuffix);
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumerLifecycleWorker(IKafkaConsumerLifecycleWorker<TKey, TValue> consumerLifecycleWorker)
        {
            if (_consumerLifecycleWorker is not null)
            {
                throw new InvalidOperationException("Consumer lifecycle worker may not be specified more than once.");
            }

            _consumerLifecycleWorker = consumerLifecycleWorker;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumeResultHandler(IConsumeResultHandler<TKey, TValue> consumeResultHandler)
        {
            if (consumeResultHandler is not null)
            {
                _consumeResultHandlers = (_consumeResultHandlers ?? []).Union([consumeResultHandler]).ToArray();
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
                _consumeResultHandlers = consumeResultHandlers.Where(consumeResultHandler => consumeResultHandler is not null).ToArray();
            }
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumeResultErrorHandler(IConsumeResultErrorHandler<TKey, TValue> consumeResultErrorHandler)
        {
            if (_consumeResultErrorHandler is not null)
            {
                throw new InvalidOperationException("Consume result error handler may not be specified more than once.");
            }

            _consumeResultErrorHandler = consumeResultErrorHandler;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithMessageOrderGuaranteeKeyHandler(Func<ConsumeResult<TKey, TValue>, object> messageOrderGuaranteeKeyHandler)
        {
            if (_messageOrderGuaranteeKeyHandler is not null)
            {
                throw new InvalidOperationException("Message order guarantee key handler may not be specified more than once.");
            }

            _messageOrderGuaranteeKeyHandler = messageOrderGuaranteeKeyHandler;
            return this;
        }

        public IKafkaConsumerWorkerBuilder<TKey, TValue> WithWorkerConfiguration(Action<IKafkaConsumerWorkerConfigBuilder> configureWorker)
        {
            if (_workerConfigured)
            {
                throw new InvalidOperationException("Worker may not be configured more than once.");
            }

            WorkerConfig = KafkaConsumerWorkerConfigBuilder.BuildConfig(Configuration, WorkerConfig, configureWorker);

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
                    [MessageOrderGuaranteeConstants.MessageOrderGuaranteeKeyHandler] = _messageOrderGuaranteeKeyHandler,
                    [KafkaIdempotencyConstants.IdempotencyHandler] = _idempotencyHandler,
                    [KafkaProducerConstants.DeadLetterProducer] = _deadLetterProducer,
                    [KafkaProducerConstants.RetryProducer] = _retryProducer,
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

            if (_consumeResultHandlers is null || !_consumeResultHandlers.Any(consumeResultHandler => consumeResultHandler is not null))
            {
                throw new InvalidOperationException("Consume result handlers cannot be null, empty, or contain null values.");
            }

            LoggerFactory ??= ServiceProvider?.GetService<ILoggerFactory>();

            _diagnosticsManager = KafkaDiagnosticsManagerFactory.Instance.GetOrCreateDiagnosticsManager(
                ServiceProvider,
                WorkerConfig.EnableDiagnostics,
                configureOptions: null);

            _builtWorker = (IKafkaConsumerWorker<TKey, TValue>)Activator.CreateInstance(DefaultConsumerWorkerType, this);

            return _builtWorker;
        }

        #endregion IKafkaConsumerWorkerBuilder Members

        #region Internal Methods

        internal static IKafkaConsumerWorker<TKey, TValue> Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerWorkerBuilder<TKey, TValue>> configureWorker,
            object workerKey)
        {
            var builder = Configure(serviceProvider, configuration, loggerFactory, configureWorker, workerKey);

            var worker = builder.Build();

            return worker;
        }

        internal static IKafkaConsumerWorkerBuilder<TKey, TValue> Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerWorkerBuilder<TKey, TValue>> configureWorker,
            object workerKey)
        {
            var builder = new KafkaConsumerWorkerBuilder<TKey, TValue>()
                .WithWorkerKey(workerKey)
                .WithConfiguration(configuration)
                .WithLoggerFactory(loggerFactory)
                .WithServiceProvider(serviceProvider);

            configureWorker?.Invoke(serviceProvider, builder);

            return builder;
        }

        #endregion Internal Methods

        #region Public Methods

        public static IKafkaConsumerWorkerBuilder<TKey, TValue> CreateBuilder(IKafkaConsumerWorkerConfig workerConfig = null)
            => new KafkaConsumerWorkerBuilder<TKey, TValue>(workerConfig);

        #endregion Public Methods
    }
}
