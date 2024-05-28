using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class SchemaRegistryJsonSerializerBuilder : ISchemaRegistryJsonSerializerBuilder
    {
        public object ClientKey { get; private set; }
        public Action<ISchemaRegistryClientBuilder> ConfigureClient { get; private set; }
        public Action<ISchemaBuilder> ConfigureSchema { get; private set; }
        public Action<IJsonSerializerConfigBuilder> ConfigureSerializer { get; private set; }
        public Action<IJsonDeserializerConfigBuilder> ConfigureDeserializer { get; private set; }
        public Action<IJsonSchemaGeneratorSettingsBuilder> ConfigureSchemaGenerator { get; private set; }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaRegistryClientConfiguration(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            ConfigureClient = configureClient;
            ClientKey = clientKey;
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaConfiguration(
            Action<ISchemaBuilder> configureSchema)
        {
            ConfigureSchema = configureSchema;
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithJsonSerializerConfiguration(
            Action<IJsonSerializerConfigBuilder> configureSerializer)
        {
            ConfigureSerializer = configureSerializer;
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithJsonDeserializerConfiguration(
            Action<IJsonDeserializerConfigBuilder> configureDeserializer)
        {
            ConfigureDeserializer = configureDeserializer;
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithJsonSchemaGeneratorConfiguration(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator)
        {
            ConfigureSchemaGenerator = configureSchemaGenerator;
            return this;
        }

        public static SchemaRegistryJsonSerializerBuilder Configure(
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryJsonSerializerBuilder();

            configureSerializer?.Invoke(builder);

            return builder;
        }
    }
}
