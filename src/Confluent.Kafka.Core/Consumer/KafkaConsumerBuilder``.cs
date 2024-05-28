using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
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
        private Func<TValue, object> _messageIdHandler;
        private IRetryHandler<TKey, TValue> _retryHandler;
        private IDiagnosticsManager _diagnosticsManager;
        private IKafkaConsumerHandlerFactory<TKey, TValue> _handlerFactory;
        private IKafkaProducer<byte[], KafkaMetadataMessage> _deadLetterProducer;
        private IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> _interceptors;

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
            : base(consumerConfig ??= BuildConfig())
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
                Interceptors = _interceptors
            };

            return _builtOptions;
        }

        #endregion IKafkaConsumerOptionsConverter Explicity Members

        #region IKafkaConsumerBuilder Members

        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IKafkaConsumerConfig ConsumerConfig { get; private set; }

        public IKafkaConsumerBuilder<TKey, TValue> WithStatisticsHandler(Action<IConsumer<TKey, TValue>, string> statisticsHandler)
        {
            SetStatisticsHandler(statisticsHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithErrorHandler(Action<IConsumer<TKey, TValue>, Error> errorHandler)
        {
            SetErrorHandler(errorHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithLogHandler(Action<IConsumer<TKey, TValue>, LogMessage> logHandler)
        {
            SetLogHandler(logHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithOAuthBearerTokenRefreshHandler(Action<IConsumer<TKey, TValue>, string> oAuthBearerTokenRefreshHandler)
        {
            SetOAuthBearerTokenRefreshHandler(oAuthBearerTokenRefreshHandler);
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
            SetPartitionsAssignedHandler(partitionsAssignedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsAssignedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartition>> partitionAssignmentHandler)
        {
            SetPartitionsAssignedHandler(partitionAssignmentHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsRevokedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            SetPartitionsRevokedHandler(partitionsRevokedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsRevokedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            SetPartitionsRevokedHandler(partitionsRevokedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsLostHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsLostHandler)
        {
            SetPartitionsLostHandler(partitionsLostHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithPartitionsLostHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsLostHandler)
        {
            SetPartitionsLostHandler(partitionsLostHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithOffsetsCommittedHandler(Action<IConsumer<TKey, TValue>, CommittedOffsets> offsetsCommittedHandler)
        {
            SetOffsetsCommittedHandler(offsetsCommittedHandler);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithConsumerKey(object consumerKey)
        {
            _consumerKey = consumerKey;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, object> messageIdHandler)
        {
            _messageIdHandler = messageIdHandler;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler)
        {
            _retryHandler = retryHandler;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithHandlerFactory(IKafkaConsumerHandlerFactory<TKey, TValue> handlerFactory)
        {
            _handlerFactory = handlerFactory;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer)
        {
            _deadLetterProducer = deadLetterProducer;
            _deadLetterProducer?.ValidateTopicSuffix(KafkaProducerConstants.DeadLetterTopicSuffix);
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> interceptors)
        {
            if (interceptors is not null && interceptors.Any(interceptor => interceptor is not null))
            {
                _interceptors = (_interceptors ?? [])
                    .Union(interceptors.Where(interceptor => interceptor is not null));
            }
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithConsumerConfiguration(Action<IServiceProvider, IKafkaConsumerConfigBuilder> configureConsumer)
        {
            BuildConfig(ServiceProvider, ConsumerConfig, configureConsumer);
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
                    [KafkaProducerConstants.DeadLetterProducer] = _deadLetterProducer,
                    [KafkaRetryConstants.RetryHandler] = _retryHandler
                }));

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

            _handlerFactory ??= KafkaConsumerHandlerFactory.GetOrCreateHandlerFactory<TKey, TValue>(
                ServiceProvider,
                LoggerFactory,
                (_, builder) => builder.WithEnableLogging(ConsumerConfig.EnableLogging),
                _consumerKey);

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

            if (PartitionsAssignedHandler is null)
            {
                SetPartitionsAssignedHandler(_handlerFactory.CreatePartitionsAssignedHandler());
            }

            if (PartitionsRevokedHandler is null)
            {
                SetPartitionsRevokedHandler(_handlerFactory.CreatePartitionsRevokedHandler());
            }

            if (PartitionsLostHandler is null)
            {
                SetPartitionsLostHandler(_handlerFactory.CreatePartitionsLostHandler());
            }

            _messageIdHandler ??= _handlerFactory.CreateMessageIdHandler();

            _interceptors ??= [];

            _diagnosticsManager ??= DiagnosticsManagerFactory.GetDiagnosticsManager(
                ServiceProvider,
                ConsumerConfig.EnableDiagnostics);

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

        #endregion IKafkaConsumerBuilder Members

        #region Public Methods

        public static IKafkaConsumerBuilder<TKey, TValue> CreateBuilder(IKafkaConsumerConfig consumerConfig = null)
            => new KafkaConsumerBuilder<TKey, TValue>(consumerConfig);

        #endregion Public Methods

        #region Private Methods

        private static IKafkaConsumerConfig BuildConfig(
            IServiceProvider serviceProvider = null,
            IKafkaConsumerConfig consumerConfig = null,
            Action<IServiceProvider, IKafkaConsumerConfigBuilder> configureConsumer = null)
        {
            using var builder = new KafkaConsumerConfigBuilder(consumerConfig);

            configureConsumer?.Invoke(serviceProvider, builder);

            return builder.Build();
        }

        #endregion Private Methods
    }
}
