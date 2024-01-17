using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry;
using Confluent.Kafka.Core.Serialization.Internal;
using Confluent.Kafka.SyncOverAsync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace Confluent.Kafka.Core.Producer
{
    public class KafkaProducerBuilder<TKey, TValue> : ProducerBuilder<TKey, TValue>, IKafkaProducerBuilder<TKey, TValue>
    {
        #region Private Fields

        private static readonly Type DefaultProducerType = typeof(KafkaProducer<TKey, TValue>);

        private Type _producerType;
        private object _producerKey;
        private Func<TValue, object> _messageIdHandler;
        private IRetryHandler<TKey, TValue> _retryHandler;
        private IDiagnosticsManager _diagnosticsManager;
        private IKafkaProducerHandlerFactory<TKey, TValue> _handlerFactory;
        private IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> _interceptors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaProducer<TKey, TValue> _builtProducer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IProducer<TKey, TValue> _builtInnerProducer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaProducerOptions<TKey, TValue> _builtOptions;

        #endregion Private Fields

        #region Ctors

        public KafkaProducerBuilder()
            : this(producerConfig: null)
        { }

        public KafkaProducerBuilder(IKafkaProducerConfig producerConfig)
            : base(producerConfig ??= BuildConfig())
        {
            ProducerConfig = producerConfig;
        }

        #endregion Ctors

        #region IKafkaProducerBuilder Explicity Members

        ILogger IKafkaProducerBuilder<TKey, TValue>.CreateLogger()
        {
            var logger = LoggerFactory.CreateLogger(ProducerConfig.EnableLogging, _producerType);

            return logger;
        }

        IProducer<TKey, TValue> IKafkaProducerBuilder<TKey, TValue>.BuildInnerProducer()
        {
            _builtInnerProducer ??= base.Build();

            return _builtInnerProducer;
        }

        IKafkaProducerOptions<TKey, TValue> IKafkaProducerBuilder<TKey, TValue>.ToOptions()
        {
            _builtOptions ??= new KafkaProducerOptions<TKey, TValue>
            {
                ProducerType = _producerType,
                LoggerFactory = LoggerFactory,
                ProducerConfig = ProducerConfig,
                DiagnosticsManager = _diagnosticsManager,
                KeySerializer = KeySerializer,
                ValueSerializer = ValueSerializer,
                MessageIdHandler = _messageIdHandler,
                RetryHandler = _retryHandler,
                Interceptors = _interceptors
            };

            return _builtOptions;
        }

        #endregion IKafkaProducerBuilder Explicity Members

        #region IKafkaProducerBuilder Members

        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IKafkaProducerConfig ProducerConfig { get; private set; }

        public IKafkaProducerBuilder<TKey, TValue> WithStatisticsHandler(Action<IProducer<TKey, TValue>, string> statisticsHandler)
        {
            SetStatisticsHandler(statisticsHandler);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithPartitioner(string topic, PartitionerDelegate partitioner)
        {
            SetPartitioner(topic, partitioner);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithDefaultPartitioner(PartitionerDelegate partitioner)
        {
            SetDefaultPartitioner(partitioner);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithErrorHandler(Action<IProducer<TKey, TValue>, Error> errorHandler)
        {
            SetErrorHandler(errorHandler);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithLogHandler(Action<IProducer<TKey, TValue>, LogMessage> logHandler)
        {
            SetLogHandler(logHandler);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithOAuthBearerTokenRefreshHandler(Action<IProducer<TKey, TValue>, string> oAuthBearerTokenRefreshHandler)
        {
            SetOAuthBearerTokenRefreshHandler(oAuthBearerTokenRefreshHandler);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithKeySerializer(ISerializer<TKey> serializer)
        {
            SetKeySerializer(serializer);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithKeySerializer(IAsyncSerializer<TKey> serializer)
        {
            SetKeySerializer(serializer?.AsSyncOverAsync());
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithValueSerializer(ISerializer<TValue> serializer)
        {
            SetValueSerializer(serializer);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithValueSerializer(IAsyncSerializer<TValue> serializer)
        {
            SetValueSerializer(serializer?.AsSyncOverAsync());
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithProducerType(Type producerType)
        {
            _producerType = producerType;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithProducerKey(object producerKey)
        {
            _producerKey = producerKey;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, object> messageIdHandler)
        {
            _messageIdHandler = messageIdHandler;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler)
        {
            _retryHandler = retryHandler;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithHandlerFactory(IKafkaProducerHandlerFactory<TKey, TValue> handlerFactory)
        {
            _handlerFactory = handlerFactory;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> interceptors)
        {
            if (interceptors is not null && interceptors.Any(interceptor => interceptor is not null))
            {
                _interceptors = (_interceptors ?? Enumerable.Empty<IKafkaProducerInterceptor<TKey, TValue>>())
                    .Union(interceptors.Where(interceptor => interceptor is not null));
            }
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithConfigureProducer(Action<IServiceProvider, IKafkaProducerConfigBuilder> configureProducer)
        {
            BuildConfig(ServiceProvider, ProducerConfig, configureProducer);
            return this;
        }

        public override IKafkaProducer<TKey, TValue> Build()
        {
            if (_builtProducer is not null)
            {
                return _builtProducer;
            }

            ProducerConfig.ValidateAndThrow<KafkaProducerConfigException>(
                new ValidationContext(ProducerConfig, new Dictionary<object, object>
                {
                    [KafkaRetryConstants.RetryHandler] = _retryHandler
                }));

            _producerType ??= DefaultProducerType;

            LoggerFactory ??= ServiceProvider?.GetService<ILoggerFactory>();

            if (KeySerializer is null)
            {
                var keySerializer = KafkaSerialization.TryGetSerializer<TKey>();

                if (keySerializer is not null)
                {
                    SetKeySerializer(keySerializer);
                }
            }

            if (ValueSerializer is null)
            {
                var valueSerializer = KafkaSerialization.TryGetSerializer<TValue>();

                if (valueSerializer is not null)
                {
                    SetValueSerializer(valueSerializer);
                }
            }

            _handlerFactory ??= KafkaProducerHandlerFactory.GetOrCreateHandlerFactory<TKey, TValue>(
                ServiceProvider,
                LoggerFactory,
                (_, builder) => builder.WithEnableLogging(ProducerConfig.EnableLogging),
                _producerKey);

            if (_handlerFactory is not null)
            {
                if (StatisticsHandler is null)
                {
                    SetStatisticsHandler(_handlerFactory.CreateStatisticsHandler());
                }

                if (ErrorHandler is null)
                {
                    SetErrorHandler(_handlerFactory.CreateErrorHandler());
                }

                if (LogHandler is null)
                {
                    SetLogHandler(_handlerFactory.CreateLogHandler());
                }

                _messageIdHandler ??= _handlerFactory.CreateMessageIdHandler();
            }

            _interceptors ??= Enumerable.Empty<IKafkaProducerInterceptor<TKey, TValue>>();

            _diagnosticsManager ??= DiagnosticsManagerFactory.GetDiagnosticsManager(
                ServiceProvider,
                ProducerConfig.EnableDiagnostics);

            _builtProducer = (IKafkaProducer<TKey, TValue>)Activator.CreateInstance(_producerType, this);

            return _builtProducer;
        }

        #endregion IKafkaProducerBuilder Members

        #region Public Methods

        public static IKafkaProducerBuilder<TKey, TValue> CreateBuilder(IKafkaProducerConfig producerConfig = null)
            => new KafkaProducerBuilder<TKey, TValue>(producerConfig);

        #endregion Public Methods

        #region Private Methods

        private static IKafkaProducerConfig BuildConfig(
            IServiceProvider serviceProvider = null,
            IKafkaProducerConfig producerConfig = null,
            Action<IServiceProvider, IKafkaProducerConfigBuilder> configureProducer = null)
        {
            using var builder = new KafkaProducerConfigBuilder(producerConfig);

            configureProducer?.Invoke(serviceProvider, builder);

            return builder.Build();
        }

        #endregion Private Methods
    }
}
