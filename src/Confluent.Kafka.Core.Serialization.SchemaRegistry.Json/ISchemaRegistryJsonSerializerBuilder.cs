using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface ISchemaRegistryJsonSerializerBuilder
    {
        ISchemaRegistryJsonSerializerBuilder WithSchemaRegistryClientConfiguration(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryJsonSerializerBuilder WithSchemaConfiguration(
            Action<ISchemaBuilder> configureSchema);

        ISchemaRegistryJsonSerializerBuilder WithJsonSerializerConfiguration(
            Action<IJsonSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryJsonSerializerBuilder WithJsonDeserializerConfiguration(
            Action<IJsonDeserializerConfigBuilder> configureDeserializer);

        ISchemaRegistryJsonSerializerBuilder WithJsonSchemaGeneratorConfiguration(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator);
    }
}
