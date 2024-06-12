using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal static class SchemaRegistryAvroSerializerFactory
    {
        public static SchemaRegistryAvroSerializer<T> GetOrCreateSerializer<T>(
           IServiceProvider serviceProvider,
           SchemaRegistryAvroSerializerBuilder builder,
           object serializerKey = null)
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryAvroSerializer<T>>(
                serializerKey ?? SchemaRegistryAvroSerializerConstants.SchemaRegistryAvroSerializerKey) ??
                CreateSerializer<T>(serviceProvider, builder);

            return serializer;
        }

        public static SchemaRegistryAvroSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            SchemaRegistryAvroSerializerBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var schemaRegistryClient = SchemaRegistryClientFactory.GetOrCreateSchemaRegistryClient(
                serviceProvider,
                builder.ConfigureClient,
                builder.ClientKey);

            var serializerConfig = AvroSerializerConfigBuilder.Build(builder.ConfigureSerializer);

            var deserializerConfig = AvroDeserializerConfigBuilder.Build(builder.ConfigureDeserializer);

            var serializer = new SchemaRegistryAvroSerializer<T>(
                schemaRegistryClient,
                serializerConfig,
                deserializerConfig);

            return serializer;
        }
    }
}
