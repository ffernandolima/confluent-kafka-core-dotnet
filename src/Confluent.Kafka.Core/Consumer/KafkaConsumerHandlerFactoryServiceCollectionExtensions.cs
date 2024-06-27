using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Consumer.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaConsumerHandlerFactoryServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions = null,
            object consumerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.AddKafkaConsumerHandlerFactory(serviceProvider =>
            {
                var handlerFactory = KafkaConsumerHandlerFactory.Instance.CreateHandlerFactory<TKey, TValue>(
                    serviceProvider,
                    serviceProvider.GetService<IConfiguration>(),
                    serviceProvider.GetService<ILoggerFactory>(),
                    configureOptions);

                return handlerFactory;
            },
            consumerKey);

            return services;
        }

        public static IServiceCollection AddKafkaConsumerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            Func<IServiceProvider, IKafkaConsumerHandlerFactory<TKey, TValue>> implementationFactory,
            object consumerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory), $"{nameof(implementationFactory)} cannot be null.");
            }

            services.TryAddKeyedSingleton(consumerKey, (serviceProvider, _) => implementationFactory.Invoke(serviceProvider));

            return services;
        }
    }
}
