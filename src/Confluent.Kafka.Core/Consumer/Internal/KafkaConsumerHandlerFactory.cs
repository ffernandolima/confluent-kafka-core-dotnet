using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class KafkaConsumerHandlerFactory
    {
        public static IKafkaConsumerHandlerFactory<TKey, TValue> GetOrCreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory = null,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions = null,
            object consumerKey = null)
        {
            var handlerFactory = serviceProvider?.GetKeyedService<IKafkaConsumerHandlerFactory<TKey, TValue>>(consumerKey) ??
                CreateHandlerFactory<TKey, TValue>(
                    serviceProvider,
                    loggerFactory,
                    configureOptions);

            return handlerFactory;
        }

        public static IKafkaConsumerHandlerFactory<TKey, TValue> CreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory = null,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions = null)
        {
            var options = KafkaConsumerHandlerFactoryOptionsBuilder.Build(
                serviceProvider,
                configureOptions);

            var handlerFactory = new KafkaConsumerHandlerFactory<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                options);

            return handlerFactory;
        }
    }
}
