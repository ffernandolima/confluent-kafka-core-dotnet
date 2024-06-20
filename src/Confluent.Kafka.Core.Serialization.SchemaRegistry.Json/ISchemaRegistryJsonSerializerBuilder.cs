using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface ISchemaRegistryJsonSerializerBuilder
    {
        ISchemaRegistryJsonSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryJsonSerializerBuilder WithSchema(
            Action<ISchemaBuilder> configureSchema);

        ISchemaRegistryJsonSerializerBuilder WithSerializerConfiguration(
            Action<IJsonSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryJsonSerializerBuilder WithDeserializerConfiguration(
            Action<IJsonDeserializerConfigBuilder> configureDeserializer);

        ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorSettings(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator);
    }
}
