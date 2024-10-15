using Confluent.SchemaRegistry;
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

#if NET8_0_OR_GREATER
        ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorSettings(
            Action<INewtonsoftJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator);
#else
        ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorSettings(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator);
#endif
        ISchemaRegistryJsonSerializerBuilder WithRuleRegistry(
            RuleRegistry ruleRegistry);
    }
}
