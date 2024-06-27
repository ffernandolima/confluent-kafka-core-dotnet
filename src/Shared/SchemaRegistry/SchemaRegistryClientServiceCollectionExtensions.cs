using Confluent.Kafka.Core.Serialization.SchemaRegistry;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaRegistryClientServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaRegistryClient(
            this IServiceCollection services,
            Action<IServiceProvider, ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureClient is null)
            {
                throw new ArgumentNullException(nameof(configureClient), $"{nameof(configureClient)} cannot be null.");
            }

            services.TryAddKeyedSingleton(
                clientKey ?? SchemaRegistryClientConstants.ConfluentSchemaRegistryClientKey,
                (serviceProvider, _) =>
                {
                    var schemaRegistryClient = SchemaRegistryClientFactory.Instance.CreateSchemaRegistryClient(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        configureClient);

                    return schemaRegistryClient;
                });

            return services;
        }
    }
}
