using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf
{
    public interface ISchemaRegistryProtobufSerializerBuilder
    {
        ISchemaRegistryProtobufSerializerBuilder WithSchemaRegistryClientConfiguration(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryProtobufSerializerBuilder WithProtobufSerializerConfiguration(
            Action<IProtobufSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryProtobufSerializerBuilder WithProtobufDeserializerConfiguration(
            Action<IProtobufDeserializerConfigBuilder> configureDeserializer);
    }
}
