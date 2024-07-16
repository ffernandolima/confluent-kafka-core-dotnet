using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaRegistryAvroSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaRegistryAvroSerializer<T>(
            this IServiceCollection services,
            Action<IServiceProvider, ISchemaRegistryAvroSerializerBuilder> configureSerializer,
            object serializerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer));
            }

            var serviceKey = serializerKey ?? SchemaRegistryAvroSerializerConstants.SchemaRegistryAvroSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) =>
                {
                    var serializer = SchemaRegistryAvroSerializerFactory.Instance.CreateSerializer<T>(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        configureSerializer);

                    return serializer;
                });

            services.TryAddKeyedSingleton<IAsyncSerializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<SchemaRegistryAvroSerializer<T>>(serviceKey));

            services.TryAddKeyedSingleton<IAsyncDeserializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<SchemaRegistryAvroSerializer<T>>(serviceKey));

            return services;
        }
    }
}
