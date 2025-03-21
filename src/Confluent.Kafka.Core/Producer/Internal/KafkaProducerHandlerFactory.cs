﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerHandlerFactory : IKafkaProducerHandlerFactory
    {
        private static readonly Lazy<KafkaProducerHandlerFactory> Factory = new(
            () => new KafkaProducerHandlerFactory(), isThreadSafe: true);

        public static KafkaProducerHandlerFactory Instance => Factory.Value;

        private KafkaProducerHandlerFactory()
        { }

        public IKafkaProducerHandlerFactory<TKey, TValue> GetOrCreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions,
            object producerKey)
        {
            var handlerFactory = serviceProvider?.GetKeyedService<IKafkaProducerHandlerFactory<TKey, TValue>>(producerKey) ??
                CreateHandlerFactory<TKey, TValue>(
                    serviceProvider,
                    configuration,
                    loggerFactory,
                    (_, builder) => configureOptions?.Invoke(builder));

            return handlerFactory;
        }

        public IKafkaProducerHandlerFactory<TKey, TValue> CreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions)
        {
            var options = KafkaProducerHandlerFactoryOptionsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureOptions);

            var handlerFactory = new KafkaProducerHandlerFactory<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                options);

            return handlerFactory;
        }
    }
}
