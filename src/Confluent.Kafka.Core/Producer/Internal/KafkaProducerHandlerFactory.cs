using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal static class KafkaProducerHandlerFactory
    {
        public static IKafkaProducerHandlerFactory<TKey, TValue> GetOrCreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory = null,
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions = null,
            object producerKey = null)
        {
            var handlerFactory = serviceProvider?.GetKeyedService<IKafkaProducerHandlerFactory<TKey, TValue>>(producerKey) ??
                CreateHandlerFactory<TKey, TValue>(
                    serviceProvider,
                    loggerFactory,
                    configureOptions);

            return handlerFactory;
        }

        public static IKafkaProducerHandlerFactory<TKey, TValue> CreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory = null,
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions = null)
        {
            var options = KafkaProducerHandlerFactoryOptionsBuilder.Build(
                serviceProvider,
                configureOptions);

            var handlerFactory = new KafkaProducerHandlerFactory<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                options);

            return handlerFactory;
        }
    }
}
