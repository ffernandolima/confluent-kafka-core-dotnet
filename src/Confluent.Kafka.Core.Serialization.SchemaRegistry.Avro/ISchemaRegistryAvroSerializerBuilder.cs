using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
{
    public interface ISchemaRegistryAvroSerializerBuilder
    {
        ISchemaRegistryAvroSerializerBuilder WithSchemaRegistryClientConfiguration(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryAvroSerializerBuilder WithAvroSerializerConfiguration(
            Action<IAvroSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryAvroSerializerBuilder WithAvroDeserializerConfiguration(
            Action<IAvroDeserializerConfigBuilder> configureDeserializer);
    }
}
