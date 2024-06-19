using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal static class SchemaRegistryJsonSerializerFactory
    {
        public static SchemaRegistryJsonSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer,
            object serializerKey)
                where T : class
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryJsonSerializer<T>>(
                serializerKey ?? SchemaRegistryJsonSerializerConstants.SchemaRegistryJsonSerializerKey) ??
                CreateSerializer<T>(serviceProvider, (_, builder) => configureSerializer?.Invoke(builder));

            return serializer;
        }

        public static SchemaRegistryJsonSerializer<T> CreateSerializer<T>(
           IServiceProvider serviceProvider,
           Action<IServiceProvider, ISchemaRegistryJsonSerializerBuilder> configureSerializer)
                where T : class
        {
            var builder = SchemaRegistryJsonSerializerBuilder.Configure(serviceProvider, configureSerializer);

            var serializer = new SchemaRegistryJsonSerializer<T>(
                builder.SchemaRegistryClient,
                builder.RegisteredSchema ?? builder.UnregisteredSchema,
                builder.SerializerConfig,
                builder.DeserializerConfig,
                builder.SchemaGeneratorSettings);

            return serializer;
        }
    }
}
