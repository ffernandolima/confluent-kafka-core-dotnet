using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry;
using Confluent.Kafka.Core.Retry.Internal;
using Confluent.Kafka.Core.Serialization.Internal;
using Confluent.Kafka.SyncOverAsync;
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

namespace Confluent.Kafka.Core.Producer
{
    public class KafkaProducerBuilder<TKey, TValue> : ProducerBuilder<TKey, TValue>,
        IProducerBuilder<TKey, TValue>,
        IKafkaProducerBuilder<TKey, TValue>,
        IKafkaProducerOptionsConverter<TKey, TValue>
    {
        #region Private Fields

        private static readonly Type DefaultProducerType = typeof(KafkaProducer<TKey, TValue>);

        private object _producerKey;
        private bool _producerConfigured;
        private Func<TValue, object> _messageIdHandler;
        private IRetryHandler<TKey, TValue> _retryHandler;
        private IKafkaDiagnosticsManager _diagnosticsManager;
        private IKafkaProducerHandlerFactory<TKey, TValue> _handlerFactory;
        private IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> _interceptors;
        private Action<IClient, string> _oAuthBearerTokenRefreshHandler;
        private ICollection<Action<IProducer<TKey, TValue>, string>> _statisticsHandlers;
        private ICollection<Action<IProducer<TKey, TValue>, Error>> _errorHandlers;
        private ICollection<Action<IProducer<TKey, TValue>, LogMessage>> _logHandlers;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaProducer<TKey, TValue> _builtProducer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IProducer<TKey, TValue> _builtUnderlyingProducer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaProducerOptions<TKey, TValue> _builtOptions;

        #endregion Private Fields

        #region Ctors

        public KafkaProducerBuilder()
            : this(producerConfig: null)
        { }

        public KafkaProducerBuilder(IKafkaProducerConfig producerConfig)
            : base(producerConfig ??= KafkaProducerConfigBuilder.BuildConfig())
        {
            ProducerConfig = producerConfig;
        }

        #endregion Ctors

        #region IProducerBuilder Explicity Members

        IProducer<TKey, TValue> IProducerBuilder<TKey, TValue>.Build()
        {
            _builtUnderlyingProducer ??= base.Build();

            return _builtUnderlyingProducer;
        }

        #endregion IProducerBuilder Explicity Members

        #region IKafkaProducerOptionsConverter Explicity Members

        IKafkaProducerOptions<TKey, TValue> IKafkaProducerOptionsConverter<TKey, TValue>.ToOptions()
        {
            _builtOptions ??= new KafkaProducerOptions<TKey, TValue>
            {
                ProducerType = DefaultProducerType,
                LoggerFactory = LoggerFactory,
                ProducerConfig = ProducerConfig,
                DiagnosticsManager = _diagnosticsManager,
                KeySerializer = KeySerializer,
                ValueSerializer = ValueSerializer,
                MessageIdHandler = _messageIdHandler,
                RetryHandler = _retryHandler,
                Interceptors = _interceptors,
                OAuthBearerTokenRefreshHandler = _oAuthBearerTokenRefreshHandler,
                StatisticsHandlers = _statisticsHandlers,
                ErrorHandlers = _errorHandlers,
                LogHandlers = _logHandlers
            };

            return _builtOptions;
        }

        #endregion IKafkaProducerOptionsConverter Explicity Members

        #region IKafkaProducerBuilder Members

        public IConfiguration Configuration { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IKafkaProducerConfig ProducerConfig { get; private set; }

        public IKafkaProducerBuilder<TKey, TValue> WithStatisticsHandler(Action<IProducer<TKey, TValue>, string> statisticsHandler)
        {
            AttachStatisticsHandler(statisticsHandler);
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
            AttachErrorHandler(errorHandler);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithLogHandler(Action<IProducer<TKey, TValue>, LogMessage> logHandler)
        {
            AttachLogHandler(logHandler);
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithOAuthBearerTokenRefreshHandler(Action<IClient, string> oAuthBearerTokenRefreshHandler)
        {
            if (OAuthBearerTokenRefreshHandler is not null)
            {
                throw new InvalidOperationException("OAuthBearer token refresh handler may not be specified more than once.");
            }

            OAuthBearerTokenRefreshHandler = _oAuthBearerTokenRefreshHandler = oAuthBearerTokenRefreshHandler;
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

        public IKafkaProducerBuilder<TKey, TValue> WithProducerKey(object producerKey)
        {
            if (_producerKey is not null)
            {
                throw new InvalidOperationException("Producer key may not be specified more than once.");
            }

            _producerKey = producerKey;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithConfiguration(IConfiguration configuration)
        {
            if (Configuration is not null)
            {
                throw new InvalidOperationException("Configuration may not be specified more than once.");
            }

            Configuration = configuration;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            if (LoggerFactory is not null)
            {
                throw new InvalidOperationException("Logger factory may not be specified more than once.");
            }

            LoggerFactory = loggerFactory;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider is not null)
            {
                throw new InvalidOperationException("Service provider may not be specified more than once.");
            }

            ServiceProvider = serviceProvider;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, object> messageIdHandler)
        {
            if (_messageIdHandler is not null)
            {
                throw new InvalidOperationException("Message id handler may not be specified more than once.");
            }

            _messageIdHandler = messageIdHandler;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler)
        {
            if (_retryHandler is not null)
            {
                throw new InvalidOperationException("Retry handler may not be specified more than once.");
            }

            _retryHandler = retryHandler;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithHandlerFactory(IKafkaProducerHandlerFactory<TKey, TValue> handlerFactory)
        {
            if (_handlerFactory is not null)
            {
                throw new InvalidOperationException("Handler factory may not be specified more than once.");
            }

            _handlerFactory = handlerFactory;
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithInterceptor(IKafkaProducerInterceptor<TKey, TValue> interceptor)
        {
            if (interceptor is not null)
            {
                _interceptors = (_interceptors ?? []).Union([interceptor]).ToArray();
            }
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> interceptors)
        {
            if (_interceptors is not null)
            {
                throw new InvalidOperationException("Interceptors may not be specified more than once.");
            }

            if (interceptors is not null && interceptors.Any(interceptor => interceptor is not null))
            {
                _interceptors = interceptors.Where(interceptor => interceptor is not null).ToArray();
            }
            return this;
        }

        public IKafkaProducerBuilder<TKey, TValue> WithProducerConfiguration(Action<IKafkaProducerConfigBuilder> configureProducer)
        {
            if (_producerConfigured)
            {
                throw new InvalidOperationException("Producer may not be configured more than once.");
            }

            ProducerConfig = KafkaProducerConfigBuilder.BuildConfig(Configuration, ProducerConfig, configureProducer);

            _producerConfigured = true;

            return this;
        }

#if NETSTANDARD2_0_OR_GREATER
        public override IProducer<TKey, TValue> Build()
#else
        public override IKafkaProducer<TKey, TValue> Build()
#endif
        {
            if (_builtProducer is not null)
            {
                return _builtProducer;
            }

            ProducerConfig.ValidateAndThrow<KafkaProducerConfigException>(
                new ValidationContext(ProducerConfig, new Dictionary<object, object>
                {
                    [KafkaRetryConstants.RetryHandler] = _retryHandler
                }
#if NET8_0_OR_GREATER
                .ToFrozenDictionary()));
#else
                ));
#endif

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

            _handlerFactory ??= KafkaProducerHandlerFactory.Instance.GetOrCreateHandlerFactory<TKey, TValue>(
                ServiceProvider,
                Configuration,
                LoggerFactory,
                builder => builder.WithEnableLogging(ProducerConfig.EnableLogging),
                _producerKey);

            if (StatisticsHandler is null)
            {
                AttachStatisticsHandler(_handlerFactory.CreateStatisticsHandler());
            }

            if (ErrorHandler is null)
            {
                AttachErrorHandler(_handlerFactory.CreateErrorHandler());
            }

            if (LogHandler is null)
            {
                AttachLogHandler(_handlerFactory.CreateLogHandler());
            }

            _messageIdHandler ??= _handlerFactory.CreateMessageIdHandler();

            _interceptors ??= [];

            _diagnosticsManager = KafkaDiagnosticsManagerFactory.Instance.GetOrCreateDiagnosticsManager(
                ServiceProvider,
                ProducerConfig.EnableDiagnostics,
                configureOptions: null);

            _builtProducer = (IKafkaProducer<TKey, TValue>)Activator.CreateInstance(DefaultProducerType, this);

            return _builtProducer;
        }

        private void AttachStatisticsHandler(Action<IProducer<TKey, TValue>, string> statisticsHandler)
        {
            if (StatisticsHandler is not null)
            {
                throw new InvalidOperationException("Statistics handler may not be specified more than once.");
            }

            if (statisticsHandler is not null)
            {
                _statisticsHandlers ??= [];
                _statisticsHandlers.Add(statisticsHandler);

                StatisticsHandler = (consumer, statistics) =>
                {
                    foreach (var statisticsHandler in _statisticsHandlers)
                    {
                        statisticsHandler?.Invoke(consumer, statistics);
                    }
                };
            }
        }

        private void AttachErrorHandler(Action<IProducer<TKey, TValue>, Error> errorHandler)
        {
            if (ErrorHandler is not null)
            {
                throw new InvalidOperationException("Error handler may not be specified more than once.");
            }

            if (errorHandler is not null)
            {
                _errorHandlers ??= [];
                _errorHandlers.Add(errorHandler);

                ErrorHandler = (consumer, error) =>
                {
                    foreach (var errorHandler in _errorHandlers)
                    {
                        errorHandler?.Invoke(consumer, error);
                    }
                };
            }
        }

        private void AttachLogHandler(Action<IProducer<TKey, TValue>, LogMessage> logHandler)
        {
            if (LogHandler is not null)
            {
                throw new InvalidOperationException("Log handler may not be specified more than once.");
            }

            if (logHandler is not null)
            {
                _logHandlers ??= [];
                _logHandlers.Add(logHandler);

                LogHandler = (consumer, logMessage) =>
                {
                    foreach (var logHandler in _logHandlers)
                    {
                        logHandler?.Invoke(consumer, logMessage);
                    }
                };
            }
        }

        #endregion IKafkaProducerBuilder Members

        #region Internal Methods

        internal static IKafkaProducer<TKey, TValue> Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey)
        {
            var builder = Configure(serviceProvider, configuration, loggerFactory, configureProducer, producerKey);

#if NETSTANDARD2_0_OR_GREATER
            var producer = builder.Build().ToKafkaProducer();
#else
            var producer = builder.Build();
#endif
            return producer;
        }

        internal static IKafkaProducerBuilder<TKey, TValue> Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey)
        {
            var builder = new KafkaProducerBuilder<TKey, TValue>()
                .WithProducerKey(producerKey)
                .WithConfiguration(configuration)
                .WithLoggerFactory(loggerFactory)
                .WithServiceProvider(serviceProvider);

            configureProducer?.Invoke(serviceProvider, builder);

            return builder;
        }

        #endregion Internal Methods

        #region Public Methods

        public static IKafkaProducerBuilder<TKey, TValue> CreateBuilder(IKafkaProducerConfig producerConfig = null)
            => new KafkaProducerBuilder<TKey, TValue>(producerConfig);

        #endregion Public Methods
    }
}
