using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal static class SchemaRegistryProtobufSerializerFactory
    {
        public static SchemaRegistryProtobufSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            SchemaRegistryProtobufSerializerBuilder builder,
            object serializerKey)
                where T : class, IMessage<T>, new()
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryProtobufSerializer<T>>(
                serializerKey ?? SchemaRegistryProtobufSerializerConstants.SchemaRegistryProtobufSerializerKey) ??
                CreateSerializer<T>(serviceProvider, builder);

            return serializer;
        }

        public static SchemaRegistryProtobufSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            SchemaRegistryProtobufSerializerBuilder builder)
                where T : class, IMessage<T>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var schemaRegistryClient = SchemaRegistryClientFactory.GetOrCreateSchemaRegistryClient(
                serviceProvider,
                builder.ConfigureClient,
                builder.ClientKey);

            var serializerConfig = ProtobufSerializerConfigBuilder.Build(builder.ConfigureSerializer);

            var deserializerConfig = ProtobufDeserializerConfigBuilder.Build(builder.ConfigureDeserializer);

            var serializer = new SchemaRegistryProtobufSerializer<T>(
                schemaRegistryClient,
                serializerConfig,
                deserializerConfig);

            return serializer;
        }
    }
}
