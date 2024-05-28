using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal static class SchemaRegistryJsonSerializerFactory
    {
        public static SchemaRegistryJsonSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            SchemaRegistryJsonSerializerBuilder builder,
            object serializerKey = null)
                where T : class
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryJsonSerializer<T>>(
                serializerKey ?? SchemaRegistryJsonSerializerConstants.SchemaRegistryJsonSerializerKey) ??
                CreateSerializer<T>(serviceProvider, builder);

            return serializer;
        }

        public static SchemaRegistryJsonSerializer<T> CreateSerializer<T>(
           IServiceProvider serviceProvider,
           SchemaRegistryJsonSerializerBuilder builder)
                where T : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var schemaRegistryClient = SchemaRegistryClientFactory.GetOrCreateSchemaRegistryClient(
                serviceProvider,
                builder.ConfigureClient,
                builder.ClientKey);

            var schemaBuilder = SchemaBuilder.Configure(builder.ConfigureSchema);

            var serializerConfig = JsonSerializerConfigBuilder.Build(builder.ConfigureSerializer);

            var deserializerConfig = JsonDeserializerConfigBuilder.Build(builder.ConfigureDeserializer);

            var schemaGeneratorSettings = JsonSchemaGeneratorSettingsBuilder.Build(builder.ConfigureSchemaGenerator);

            var serializer = new SchemaRegistryJsonSerializer<T>(
                schemaRegistryClient,
                schemaBuilder.RegisteredSchema ?? schemaBuilder.UnregisteredSchema,
                serializerConfig,
                deserializerConfig,
                schemaGeneratorSettings);

            return serializer;
        }
    }
}
