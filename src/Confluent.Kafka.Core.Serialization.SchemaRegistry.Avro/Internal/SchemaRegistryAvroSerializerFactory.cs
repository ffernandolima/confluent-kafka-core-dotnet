using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal static class SchemaRegistryAvroSerializerFactory
    {
        public static SchemaRegistryAvroSerializer<T> GetOrCreateSerializer<T>(
           IServiceProvider serviceProvider,
           Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer,
           object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryAvroSerializer<T>>(
                serializerKey ?? SchemaRegistryAvroSerializerConstants.SchemaRegistryAvroSerializerKey) ??
                CreateSerializer<T>(serviceProvider, (_, builder) => configureSerializer?.Invoke(builder));

            return serializer;
        }

        public static SchemaRegistryAvroSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, ISchemaRegistryAvroSerializerBuilder> configureSerializer)
        {
            var builder = SchemaRegistryAvroSerializerBuilder.Configure(serviceProvider, configureSerializer);

            var serializer = new SchemaRegistryAvroSerializer<T>(
                builder.SchemaRegistryClient,
                builder.SerializerConfig,
                builder.DeserializerConfig);

            return serializer;
        }
    }
}
