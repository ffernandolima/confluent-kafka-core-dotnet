using Confluent.Kafka.Core.Producer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaProducerHandlerFactoryServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaProducerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            IKafkaProducerHandlerFactory<TKey, TValue> implementationInstance)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.RemoveAll<IKafkaProducerHandlerFactory<TKey, TValue>>();
            services.AddSingleton(implementationInstance);

            return services;
        }

        public static IServiceCollection AddKafkaProducerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            Func<IServiceProvider, IKafkaProducerHandlerFactory<TKey, TValue>> implementationFactory)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.RemoveAll<IKafkaProducerHandlerFactory<TKey, TValue>>();
            services.AddSingleton(implementationFactory);

            return services;
        }
    }
}
