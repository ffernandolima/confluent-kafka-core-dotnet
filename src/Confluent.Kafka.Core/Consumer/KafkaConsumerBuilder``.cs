using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Producer;
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

namespace Confluent.Kafka.Core.Consumer
{
    public class KafkaConsumerBuilder<TKey, TValue> : ConsumerBuilder<TKey, TValue>,
        IConsumerBuilder<TKey, TValue>,
        IKafkaConsumerBuilder<TKey, TValue>,
        IKafkaConsumerOptionsConverter<TKey, TValue>
    {
        #region Private Fields

        private static readonly Type DefaultConsumerType = typeof(KafkaConsumer<TKey, TValue>);

        private object _consumerKey;
        private bool _consumerConfigured;
        private Func<TValue, object> _messageIdHandler;
        private IRetryHandler<TKey, TValue> _retryHandler;
        private IKafkaDiagnosticsManager _diagnosticsManager;
        private IKafkaConsumerHandlerFactory<TKey, TValue> _handlerFactory;
        private IKafkaProducer<byte[], KafkaMetadataMessage> _deadLetterProducer;
        private IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> _interceptors;
        private Action<IClient, string> _oAuthBearerTokenRefreshHandler;
        private ICollection<Action<IConsumer<TKey, TValue>, string>> _statisticsHandlers;
        private ICollection<Action<IConsumer<TKey, TValue>, Error>> _errorHandlers;
        private ICollection<Action<IConsumer<TKey, TValue>, LogMessage>> _logHandlers;
        private ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartition>>> _partitionsAssignedHandlers;
        private ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>>> _partitionsRevokedHandlers;
        private ICollection<Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>>> _partitionsLostHandlers;
        private ICollection<Action<IConsumer<TKey, TValue>, CommittedOffsets>> _offsetsCommittedHandlers;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumer<TKey, TValue> _builtConsumer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IConsumer<TKey, TValue> _builtUnderlyingConsumer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumerOptions<TKey, TValue> _builtOptions;

        #endregion Private Fields

        #region Ctors

        public KafkaConsumerBuilder()
            : this(consumerConfig: null)
        { }

        public KafkaConsumerBuilder(IKafkaConsumerConfig consumerConfig)
            : base(consumerConfig ??= KafkaConsumerConfigBuilder.BuildConfig())
        {
            ConsumerConfig = consumerConfig;
        }

        #endregion Ctors

        #region IConsumerBuilder Explicity Members

        IConsumer<TKey, TValue> IConsumerBuilder<TKey, TValue>.Build()
        {
            _builtUnderlyingConsumer ??= base.Build();

            return _builtUnderlyingConsumer;
        }

        #endregion IConsumerBuilder Explicity Members

        #region IKafkaConsumerOptionsConverter Explicity Members

        IKafkaConsumerOptions<TKey, TValue> IKafkaConsumerOptionsConverter<TKey, TValue>.ToOptions()
        {
            _builtOptions ??= new KafkaConsumerOptions<TKey, TValue>
            {
                ConsumerType = DefaultConsumerType,
                LoggerFactory = LoggerFactory,
                ConsumerConfig = ConsumerConfig,
                DiagnosticsManager = _diagnosticsManager,
                KeyDeserializer = KeyDeserializer,
                ValueDeserializer = ValueDeserializer,
                MessageIdHandler = _messageIdHandler,
                RetryHandler = _retryHandler,
                DeadLetterProducer = _deadLetterProducer,
                Interceptors = _interceptors,
                OAuthBearerTokenRefreshHandler = _oAuthBearerTokenRefreshHandler,
                StatisticsHandlers = _statisticsHandlers,
                ErrorHandlers = _errorHandlers,
                LogHandlers = _logHandlers,
                PartitionsAssignedHandlers = _partitionsAssignedHandlers,
                PartitionsRevokedHandlers = _partitionsRevokedHandlers,
                PartitionsLostHandlers = _partitionsLostHandlers,
                OffsetsCommittedHandlers = _offsetsCommittedHandlers
            };

            return _builtOptions;
        }

        #endregion IKafkaConsumerOptionsConverter Explicity Members

        #region IKafkaConsumerBuilder Members

        public IConfiguration Configuration { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IKafkaConsumerConfig ConsumerConfig { get; private set; }

        public IKafkaConsumerBuilder<TKey, TValue> WithStatisticsHandler(Action<IConsumer<TKey, TValue>, string> statisticsHandler)
        {
            AttachStatisticsHandler(statisticsHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithErrorHandler(Action<IConsumer<TKey, TValue>, Error> errorHandler)
        {
            AttachErrorHandler(errorHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithLogHandler(Action<IConsumer<TKey, TValue>, LogMessage> logHandler)
        {
            AttachLogHandler(logHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithOAuthBearerTokenRefreshHandler(Action<IClient, string> oAuthBearerTokenRefreshHandler)
        {
            if (OAuthBearerTokenRefreshHandler is not null)
            {
                throw new InvalidOperationException("OAuthBearer token refresh handler may not be specified more than once.");
            }

            OAuthBearerTokenRefreshHandler = _oAuthBearerTokenRefreshHandler = oAuthBearerTokenRefreshHandler;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithKeyDeserializer(IDeserializer<TKey> deserializer)
        {
            SetKeyDeserializer(deserializer);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithKeyDeserializer(IAsyncDeserializer<TKey> deserializer)
        {
            SetKeyDeserializer(deserializer?.AsSyncOverAsync());
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithValueDeserializer(IDeserializer<TValue> deserializer)
        {
            SetValueDeserializer(deserializer);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithValueDeserializer(IAsyncDeserializer<TValue> deserializer)
        {
            SetValueDeserializer(deserializer?.AsSyncOverAsync());
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsAssignedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartition>, IEnumerable<TopicPartitionOffset>> partitionsAssignedHandler)
        {
            AttachPartitionsAssignedHandler(partitionsAssignedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsAssignedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartition>> partitionsAssignedHandler)
        {
            AttachPartitionsAssignedHandler(partitionsAssignedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsRevokedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            AttachPartitionsRevokedHandler(partitionsRevokedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsRevokedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            AttachPartitionsRevokedHandler(partitionsRevokedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsLostHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsLostHandler)
        {
            AttachPartitionsLostHandler(partitionsLostHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsLostHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsLostHandler)
        {
            AttachPartitionsLostHandler(partitionsLostHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithOffsetsCommittedHandler(Action<IConsumer<TKey, TValue>, CommittedOffsets> offsetsCommittedHandler)
        {

            AttachOffsetsCommittedHandler(offsetsCommittedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithConsumerKey(object consumerKey)
        {
            if (_consumerKey is not null)
            {
                throw new InvalidOperationException("Consumer key may not be specified more than once.");
            }

            _consumerKey = consumerKey;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithConfiguration(IConfiguration configuration)
        {
            if (Configuration is not null)
            {
                throw new InvalidOperationException("Configuration may not be specified more than once.");
            }

            Configuration = configuration;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            if (LoggerFactory is not null)
            {
                throw new InvalidOperationException("Logger factory may not be specified more than once.");
            }

            LoggerFactory = loggerFactory;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider is not null)
            {
                throw new InvalidOperationException("Service provider may not be specified more than once.");
            }

            ServiceProvider = serviceProvider;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, object> messageIdHandler)
        {
            if (_messageIdHandler is not null)
            {
                throw new InvalidOperationException("Message id handler may not be specified more than once.");
            }

            _messageIdHandler = messageIdHandler;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler)
        {
            if (_retryHandler is not null)
            {
                throw new InvalidOperationException("Retry handler may not be specified more than once.");
            }

            _retryHandler = retryHandler;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithHandlerFactory(IKafkaConsumerHandlerFactory<TKey, TValue> handlerFactory)
        {
            if (_handlerFactory is not null)
            {
                throw new InvalidOperationException("Handler factory may not be specified more than once.");
            }

            _handlerFactory = handlerFactory;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer)
        {
            if (_deadLetterProducer is not null)
            {
                throw new InvalidOperationException("Dead letter producer may not be specified more than once.");
            }

            _deadLetterProducer = deadLetterProducer;
            _deadLetterProducer?.ValidateAndThrow(KafkaProducerConstants.DeadLetterTopicSuffix);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithInterceptor(IKafkaConsumerInterceptor<TKey, TValue> interceptor)
        {
            if (interceptor is not null)
            {
                _interceptors = (_interceptors ?? []).Union([interceptor]).ToArray();
            }
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> interceptors)
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

        public IKafkaConsumerBuilder<TKey, TValue> WithConsumerConfiguration(Action<IKafkaConsumerConfigBuilder> configureConsumer)
        {
            if (_consumerConfigured)
            {
                throw new InvalidOperationException("Consumer may not be configured more than once.");
            }

            ConsumerConfig = KafkaConsumerConfigBuilder.BuildConfig(Configuration, ConsumerConfig, configureConsumer);

            _consumerConfigured = true;

            return this;
        }

#if NETSTANDARD2_0_OR_GREATER
        public override IConsumer<TKey, TValue> Build()
#else
        public override IKafkaConsumer<TKey, TValue> Build()
#endif
        {
            if (_builtConsumer is not null)
            {
                return _builtConsumer;
            }

            ConsumerConfig.ValidateAndThrow<KafkaConsumerConfigException>(
                new ValidationContext(ConsumerConfig, new Dictionary<object, object>
                {
                    [KafkaSenderConstants.Sender] = new KafkaSender(this, KafkaSenderType.Consumer),
                    [KafkaProducerConstants.DeadLetterProducer] = _deadLetterProducer,
                    [KafkaRetryConstants.RetryHandler] = _retryHandler
                }
#if NET8_0_OR_GREATER
                .ToFrozenDictionary()));
#else
                ));
#endif
            LoggerFactory ??= ServiceProvider?.GetService<ILoggerFactory>();

            if (KeyDeserializer is null)
            {
                var keyDeserializer = KafkaSerialization.TryGetDeserializer<TKey>();

                if (keyDeserializer is not null)
                {
                    SetKeyDeserializer(keyDeserializer);
                }
            }

            if (ValueDeserializer is null)
            {
                var valueDeserializer = KafkaSerialization.TryGetDeserializer<TValue>();

                if (valueDeserializer is not null)
                {
                    SetValueDeserializer(valueDeserializer);
                }
            }

            _handlerFactory ??= KafkaConsumerHandlerFactory.Instance.GetOrCreateHandlerFactory<TKey, TValue>(
                ServiceProvider,
                Configuration,
                LoggerFactory,
                builder => builder.WithEnableLogging(ConsumerConfig.EnableLogging),
                _consumerKey);

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

            if (PartitionsAssignedHandler is null)
            {
                AttachPartitionsAssignedHandler(_handlerFactory.CreatePartitionsAssignedHandler());
            }

            if (PartitionsRevokedHandler is null)
            {
                AttachPartitionsRevokedHandler(_handlerFactory.CreatePartitionsRevokedHandler());
            }

            if (PartitionsLostHandler is null)
            {
                AttachPartitionsLostHandler(_handlerFactory.CreatePartitionsLostHandler());
            }

            if (OffsetsCommittedHandler is null)
            {
                AttachOffsetsCommittedHandler(_handlerFactory.CreateOffsetsCommittedHandler());
            }

            _messageIdHandler ??= _handlerFactory.CreateMessageIdHandler();

            _interceptors ??= [];

            _diagnosticsManager = KafkaDiagnosticsManagerFactory.Instance.GetOrCreateDiagnosticsManager(
                ServiceProvider,
                ConsumerConfig.EnableDiagnostics,
                configureOptions: null);

            _builtConsumer = (IKafkaConsumer<TKey, TValue>)Activator.CreateInstance(DefaultConsumerType, this);

            if (ConsumerConfig.HasTopicSubscriptions())
            {
                _builtConsumer.Subscribe(ConsumerConfig.TopicSubscriptions);
            }

            if (ConsumerConfig.HasPartitionAssignments())
            {
                _builtConsumer.Assign(ConsumerConfig.PartitionAssignments);
            }

            return _builtConsumer;
        }

        private void AttachStatisticsHandler(Action<IConsumer<TKey, TValue>, string> statisticsHandler)
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

        private void AttachErrorHandler(Action<IConsumer<TKey, TValue>, Error> errorHandler)
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

        private void AttachLogHandler(Action<IConsumer<TKey, TValue>, LogMessage> logHandler)
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

        private void AttachPartitionsAssignedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartition>> partitionsAssignedHandler)
        {
            if (PartitionsAssignedHandler is not null)
            {
                throw new InvalidOperationException("Partitions assigned handler may not be specified more than once.");
            }

            if (partitionsAssignedHandler is not null)
            {
                _partitionsAssignedHandlers ??= [];
                _partitionsAssignedHandlers.Add(partitionsAssignedHandler);

                PartitionsAssignedHandler = (consumer, assignments) =>
                {
                    foreach (var partitionsAssignedHandler in _partitionsAssignedHandlers)
                    {
                        partitionsAssignedHandler?.Invoke(consumer, assignments);
                    }

                    return assignments.Select(assignment => new TopicPartitionOffset(assignment, Offset.Unset));
                };
            }
        }

        private void AttachPartitionsAssignedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartition>, IEnumerable<TopicPartitionOffset>> partitionsAssignedHandler)
        {
            if (PartitionsAssignedHandler is not null)
            {
                throw new InvalidOperationException("Partitions assigned handler may not be specified more than once.");
            }

            if (partitionsAssignedHandler is not null)
            {
                IEnumerable<TopicPartitionOffset> offsetAssignments = null;

                _partitionsAssignedHandlers ??= [];
                _partitionsAssignedHandlers.Add((consumer, assignments) =>
                {
                    offsetAssignments = partitionsAssignedHandler.Invoke(consumer, assignments);
                });

                PartitionsAssignedHandler = (consumer, assignments) =>
                {
                    foreach (var partitionsAssignedHandler in _partitionsAssignedHandlers)
                    {
                        partitionsAssignedHandler?.Invoke(consumer, assignments);
                    }

                    return offsetAssignments;
                };
            }
        }

        private void AttachPartitionsRevokedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            if (PartitionsRevokedHandler is not null)
            {
                throw new InvalidOperationException("Partitions revoked handler may not be specified more than once.");
            }

            if (partitionsRevokedHandler is not null)
            {
                _partitionsRevokedHandlers ??= [];
                _partitionsRevokedHandlers.Add(partitionsRevokedHandler);

                PartitionsRevokedHandler = (consumer, revokements) =>
                {
                    foreach (var partitionsRevokedHandler in _partitionsRevokedHandlers)
                    {
                        partitionsRevokedHandler?.Invoke(consumer, revokements);
                    }

                    return [];
                };
            }
        }

        private void AttachPartitionsRevokedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            if (PartitionsRevokedHandler is not null)
            {
                throw new InvalidOperationException("Partitions revoked handler may not be specified more than once.");
            }

            if (partitionsRevokedHandler is not null)
            {
                IEnumerable<TopicPartitionOffset> offsetRevokements = null;

                _partitionsRevokedHandlers ??= [];
                _partitionsRevokedHandlers.Add((consumer, revokements) =>
                {
                    offsetRevokements = partitionsRevokedHandler.Invoke(consumer, revokements);
                });

                PartitionsRevokedHandler = (consumer, revokements) =>
                {
                    foreach (var partitionsRevokedHandler in _partitionsRevokedHandlers)
                    {
                        partitionsRevokedHandler?.Invoke(consumer, revokements);
                    }

                    return offsetRevokements;
                };

                RevokedOrLostHandlerIsFunc = true;
            }
        }

        private void AttachPartitionsLostHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsLostHandler)
        {
            if (PartitionsLostHandler is not null)
            {
                throw new InvalidOperationException("Partitions lost handler may not be specified more than once.");
            }

            if (partitionsLostHandler is not null)
            {
                _partitionsLostHandlers ??= [];
                _partitionsLostHandlers.Add(partitionsLostHandler);

                PartitionsLostHandler = (consumer, losses) =>
                {
                    foreach (var partitionsLostHandler in _partitionsLostHandlers)
                    {
                        partitionsLostHandler?.Invoke(consumer, losses);
                    }

                    return [];
                };
            }
        }

        private void AttachPartitionsLostHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsLostHandler)
        {
            if (PartitionsLostHandler is not null)
            {
                throw new InvalidOperationException("Partitions lost handler may not be specified more than once.");
            }

            if (partitionsLostHandler is not null)
            {
                IEnumerable<TopicPartitionOffset> offsetLosses = null;

                _partitionsLostHandlers ??= [];
                _partitionsLostHandlers.Add((consumer, losses) =>
                {
                    offsetLosses = partitionsLostHandler.Invoke(consumer, losses);
                });

                PartitionsLostHandler = (consumer, losses) =>
                {
                    foreach (var partitionsLostHandler in _partitionsLostHandlers)
                    {
                        partitionsLostHandler?.Invoke(consumer, losses);
                    }

                    return offsetLosses;
                };

                RevokedOrLostHandlerIsFunc = true;
            }
        }

        private void AttachOffsetsCommittedHandler(Action<IConsumer<TKey, TValue>, CommittedOffsets> offsetsCommittedHandler)
        {
            if (OffsetsCommittedHandler is not null)
            {
                throw new InvalidOperationException("Offsets committed handler may not be specified more than once.");
            }

            if (offsetsCommittedHandler is not null)
            {
                _offsetsCommittedHandlers ??= [];
                _offsetsCommittedHandlers.Add(offsetsCommittedHandler);

                OffsetsCommittedHandler = (consumer, committedOffsets) =>
                {
                    foreach (var offsetsCommittedHandler in _offsetsCommittedHandlers)
                    {
                        offsetsCommittedHandler?.Invoke(consumer, committedOffsets);
                    }
                };
            }
        }

        #endregion IKafkaConsumerBuilder Members

        #region Internal Methods

        internal static IKafkaConsumer<TKey, TValue> Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey)
        {
            var builder = Configure(serviceProvider, configuration, loggerFactory, configureConsumer, consumerKey);

#if NETSTANDARD2_0_OR_GREATER
            var consumer = builder.Build().ToKafkaConsumer();
#else
            var consumer = builder.Build();
#endif
            return consumer;
        }

        internal static IKafkaConsumerBuilder<TKey, TValue> Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey)
        {
            var builder = new KafkaConsumerBuilder<TKey, TValue>()
                .WithConsumerKey(consumerKey)
                .WithConfiguration(configuration)
                .WithLoggerFactory(loggerFactory)
                .WithServiceProvider(serviceProvider);

            configureConsumer?.Invoke(serviceProvider, builder);

            return builder;
        }

        #endregion Internal Methods

        #region Public Methods

        public static IKafkaConsumerBuilder<TKey, TValue> CreateBuilder(IKafkaConsumerConfig consumerConfig = null)
            => new KafkaConsumerBuilder<TKey, TValue>(consumerConfig);

        #endregion Public Methods
    }
}
