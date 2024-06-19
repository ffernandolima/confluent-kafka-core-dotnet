using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal static class SchemaRegistryProtobufSerializerFactory
    {
        public static SchemaRegistryProtobufSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer,
            object serializerKey)
                where T : class, IMessage<T>, new()
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryProtobufSerializer<T>>(
                serializerKey ?? SchemaRegistryProtobufSerializerConstants.SchemaRegistryProtobufSerializerKey) ??
                CreateSerializer<T>(serviceProvider, (_, builder) => configureSerializer?.Invoke(builder));

            return serializer;
        }

        public static SchemaRegistryProtobufSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, ISchemaRegistryProtobufSerializerBuilder> configureSerializer)
                where T : class, IMessage<T>, new()
        {
            var builder = SchemaRegistryProtobufSerializerBuilder.Configure(serviceProvider, configureSerializer);

            var serializer = new SchemaRegistryProtobufSerializer<T>(
                builder.SchemaRegistryClient,
                builder.SerializerConfig,
                builder.DeserializerConfig);

            return serializer;
        }
    }
}
