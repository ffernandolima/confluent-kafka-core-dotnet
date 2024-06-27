using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerHandlerFactory
    {
        private static readonly Lazy<KafkaConsumerHandlerFactory> Factory = new(
            () => new KafkaConsumerHandlerFactory(), isThreadSafe: true);

        public static KafkaConsumerHandlerFactory Instance => Factory.Value;

        private KafkaConsumerHandlerFactory()
        { }

        public IKafkaConsumerHandlerFactory<TKey, TValue> GetOrCreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions,
            object consumerKey)
        {
            var handlerFactory = serviceProvider?.GetKeyedService<IKafkaConsumerHandlerFactory<TKey, TValue>>(consumerKey) ??
                CreateHandlerFactory<TKey, TValue>(
                    serviceProvider,
                    configuration,
                    loggerFactory,
                    (_, builder) => configureOptions?.Invoke(builder));

            return handlerFactory;
        }

        public IKafkaConsumerHandlerFactory<TKey, TValue> CreateHandlerFactory<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions)
        {
            var options = KafkaConsumerHandlerFactoryOptionsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureOptions);

            var handlerFactory = new KafkaConsumerHandlerFactory<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                options);

            return handlerFactory;
        }
    }
}
