using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface ISchemaRegistryJsonSerializerBuilder
    {
        ISchemaRegistryJsonSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryJsonSerializerBuilder WithSchemaConfiguration(
            Action<ISchemaBuilder> configureSchema);

        ISchemaRegistryJsonSerializerBuilder WithSerializerConfiguration(
            Action<IJsonSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryJsonSerializerBuilder WithDeserializerConfiguration(
            Action<IJsonDeserializerConfigBuilder> configureDeserializer);

        ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorConfiguration(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator);
    }
}
