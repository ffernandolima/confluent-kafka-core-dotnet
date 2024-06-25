using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaProducerHandlerFactoryServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaProducerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions = null,
            object producerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.AddKafkaProducerHandlerFactory(serviceProvider =>
            {
                var handlerFactory = KafkaProducerHandlerFactory.CreateHandlerFactory<TKey, TValue>(
                    serviceProvider,
                    serviceProvider.GetService<IConfiguration>(),
                    serviceProvider.GetService<ILoggerFactory>(),
                    configureOptions);

                return handlerFactory;
            },
            producerKey);

            return services;
        }

        public static IServiceCollection AddKafkaProducerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            Func<IServiceProvider, IKafkaProducerHandlerFactory<TKey, TValue>> implementationFactory,
            object producerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory), $"{nameof(implementationFactory)} cannot be null.");
            }

            services.TryAddKeyedSingleton(producerKey, (serviceProvider, _) => implementationFactory.Invoke(serviceProvider));

            return services;
        }
    }
}
