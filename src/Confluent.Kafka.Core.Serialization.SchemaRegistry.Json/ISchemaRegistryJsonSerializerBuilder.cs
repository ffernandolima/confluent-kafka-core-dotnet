using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface ISchemaRegistryJsonSerializerBuilder
    {
        ISchemaRegistryJsonSerializerBuilder WithConfigureSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryJsonSerializerBuilder WithConfigureSchema(
            Action<ISchemaBuilder> configureSchema);

        ISchemaRegistryJsonSerializerBuilder WithConfigureJsonSerializer(
            Action<IJsonSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryJsonSerializerBuilder WithConfigureJsonDeserializer(
            Action<IJsonDeserializerConfigBuilder> configureDeserializer);

        ISchemaRegistryJsonSerializerBuilder WithConfigureJsonSchemaGenerator(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator);
    }
}
