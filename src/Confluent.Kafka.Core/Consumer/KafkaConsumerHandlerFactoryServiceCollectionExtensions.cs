using Confluent.Kafka.Core.Consumer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaConsumerHandlerFactoryServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            IKafkaConsumerHandlerFactory<TKey, TValue> implementationInstance)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.RemoveAll<IKafkaConsumerHandlerFactory<TKey, TValue>>();
            services.AddSingleton(implementationInstance);

            return services;
        }

        public static IServiceCollection AddKafkaConsumerHandlerFactory<TKey, TValue>(
            this IServiceCollection services,
            Func<IServiceProvider, IKafkaConsumerHandlerFactory<TKey, TValue>> implementationFactory)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.RemoveAll<IKafkaConsumerHandlerFactory<TKey, TValue>>();
            services.AddSingleton(implementationFactory);

            return services;
        }
    }
}
