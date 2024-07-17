using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal interface ISchemaRegistryProtobufSerializerFactory
    {
        SchemaRegistryProtobufSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer,
            object serializerKey)
                where T : class, IMessage<T>, new();

        SchemaRegistryProtobufSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryProtobufSerializerBuilder> configureSerializer)
                where T : class, IMessage<T>, new();
    }
}
