using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaRegistryAvroSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaRegistryAvroSerializer<T>(
            this IServiceCollection services,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer,
            object serializerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer), $"{nameof(configureSerializer)} cannot be null.");
            }

            var builder = SchemaRegistryAvroSerializerBuilder.Configure(configureSerializer);

            services.AddSchemaRegistryClient(builder.ConfigureClient, builder.ClientKey);

            var serviceKey = serializerKey ?? SchemaRegistryAvroSerializerConstants.SchemaRegistryAvroSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) => SchemaRegistryAvroSerializerFactory.CreateSerializer<T>(serviceProvider, builder));

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
