using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaRegistryJsonSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaRegistryJsonSerializer<T>(
            this IServiceCollection services,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer,
            object serializerKey = null)
                where T : class
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer), $"{nameof(configureSerializer)} cannot be null.");
            }

            var builder = SchemaRegistryJsonSerializerBuilder.Configure(configureSerializer);

            services.AddSchemaRegistryClient(builder.ConfigureClient, builder.ClientKey);

            var serviceKey = serializerKey ?? SchemaRegistryJsonSerializerConstants.SchemaRegistryJsonSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) => SchemaRegistryJsonSerializerFactory.CreateSerializer<T>(serviceProvider, builder));

            services.TryAddKeyedSingleton<IAsyncSerializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<SchemaRegistryJsonSerializer<T>>(serviceKey));

            services.TryAddKeyedSingleton<IAsyncDeserializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<SchemaRegistryJsonSerializer<T>>(serviceKey));

            return services;
        }
    }
}
