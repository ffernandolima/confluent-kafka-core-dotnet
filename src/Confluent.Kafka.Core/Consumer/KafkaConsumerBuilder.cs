﻿using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Internal;
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
    public class KafkaConsumerBuilder<TKey, TValue> : ConsumerBuilder<TKey, TValue>, IKafkaConsumerBuilder<TKey, TValue>
    {
        #region Private Fields

        private static readonly Type DefaultConsumerType = typeof(KafkaConsumer<TKey, TValue>);

        private readonly IKafkaConsumerConfig _consumerConfig;

        private Type _consumerType;
        private Func<TValue, object> _messageIdHandler;
        private IRetryHandler<TKey, TValue> _retryHandler;
        private IKafkaConsumerHandlerFactory<TKey, TValue> _handlerFactory;
        private Func<IKafkaConsumer<TKey, TValue>, object> _consumerIdHandler;
        private IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> _interceptors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IKafkaConsumer<TKey, TValue> _builtConsumer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IConsumer<TKey, TValue> _builtInnerConsumer;

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
            _consumerConfig = consumerConfig;
        }

        #endregion Ctors

        #region IKafkaConsumerBuilder Explicity Members

        ILogger IKafkaConsumerBuilder<TKey, TValue>.CreateLogger()
        {
            var logger = LoggerFactory.CreateLogger(_consumerConfig.EnableLogging, _consumerType);

            return logger;
        }

        IConsumer<TKey, TValue> IKafkaConsumerBuilder<TKey, TValue>.BuildInnerConsumer()
        {
            _builtInnerConsumer ??= base.Build();

            return _builtInnerConsumer;
        }

        IKafkaConsumerOptions<TKey, TValue> IKafkaConsumerBuilder<TKey, TValue>.ToOptions()
        {
            _builtOptions ??= new KafkaConsumerOptions<TKey, TValue>
            {
                ConsumerType = _consumerType,
                LoggerFactory = LoggerFactory,
                ConsumerConfig = _consumerConfig,
                KeyDeserializer = KeyDeserializer,
                ValueDeserializer = ValueDeserializer,
                ConsumerIdHandler = _consumerIdHandler,
                MessageIdHandler = _messageIdHandler,
                RetryHandler = _retryHandler,
                Interceptors = _interceptors
            };

            return _builtOptions;
        }

        #endregion IKafkaConsumerBuilder Explicity Members

        #region IKafkaConsumerBuilder Members

        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }

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

        public IKafkaConsumerBuilder<TKey, TValue> WithConsumerType(Type consumerType)
        {
            _consumerType = consumerType;
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

        public IKafkaConsumerBuilder<TKey, TValue> WithConsumerIdHandler(Func<IKafkaConsumer<TKey, TValue>, object> consumerIdHandler)
        {
            _consumerIdHandler = consumerIdHandler;
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

        public IKafkaConsumerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> interceptors)
        {
            if (interceptors is not null && interceptors.Any())
            {
                _interceptors = (_interceptors ?? Enumerable.Empty<IKafkaConsumerInterceptor<TKey, TValue>>())
                    .Union(interceptors.Where(interceptor => interceptor is not null));
            }
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithHandlerFactory(IKafkaConsumerHandlerFactory<TKey, TValue> handlerFactory)
        {
            _handlerFactory = handlerFactory;
            return this;
        }

        public IKafkaConsumerBuilder<TKey, TValue> WithConfigureConsumer(Action<IServiceProvider, IKafkaConsumerConfigBuilder> configureConsumer)
        {
            BuildConfig(ServiceProvider, _consumerConfig, configureConsumer);
            return this;
        }

        public override IKafkaConsumer<TKey, TValue> Build()
        {
            if (_builtConsumer is not null)
            {
                return _builtConsumer;
            }

            _consumerConfig.ValidateAndThrow<KafkaConsumerConfigException>(
                new ValidationContext(_consumerConfig, new Dictionary<object, object>
                {
                    ["RetryHandler"] = _retryHandler
                }));

            _consumerType ??= DefaultConsumerType;

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

            _interceptors ??= Enumerable.Empty<IKafkaConsumerInterceptor<TKey, TValue>>();

            _handlerFactory ??= ServiceProvider?.GetService<IKafkaConsumerHandlerFactory<TKey, TValue>>();

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

                _consumerIdHandler ??= _handlerFactory.CreateConsumerIdHandler();

                _messageIdHandler ??= _handlerFactory.CreateMessageIdHandler();
            }

            _builtConsumer = (IKafkaConsumer<TKey, TValue>)Activator.CreateInstance(_consumerType, this);

            if (_consumerConfig.HasTopicSubscriptions)
            {
                _builtConsumer.Subscribe(_consumerConfig.TopicSubscriptions.Where(topic => !string.IsNullOrWhiteSpace(topic)));
            }

            if (_consumerConfig.HasPartitionAssignments)
            {
                _builtConsumer.Assign(_consumerConfig.PartitionAssignments.Where(partition => partition is not null));
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
