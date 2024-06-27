using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaRegistryJsonSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaRegistryJsonSerializer<T>(
            this IServiceCollection services,
            Action<IServiceProvider, ISchemaRegistryJsonSerializerBuilder> configureSerializer,
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

            var serviceKey = serializerKey ?? SchemaRegistryJsonSerializerConstants.SchemaRegistryJsonSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) =>
                {
                    var serializer = SchemaRegistryJsonSerializerFactory.Instance.CreateSerializer<T>(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        configureSerializer);

                    return serializer;
                });

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
