using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class KafkaConsumerHandlerFactory
    {
        public static IKafkaConsumerHandlerFactory<TKey, TValue> GetOrCreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            Action<IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions,
            object consumerKey)
        {
            var handlerFactory = serviceProvider?.GetKeyedService<IKafkaConsumerHandlerFactory<TKey, TValue>>(consumerKey) ??
                CreateHandlerFactory<TKey, TValue>(serviceProvider, loggerFactory, (_, builder) => configureOptions?.Invoke(builder));

            return handlerFactory;
        }

        public static IKafkaConsumerHandlerFactory<TKey, TValue> CreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions)
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
