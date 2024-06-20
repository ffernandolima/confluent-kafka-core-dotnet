using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using NJsonSchema.Generation;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class SchemaRegistryJsonSerializerBuilder : ISchemaRegistryJsonSerializerBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public ISchemaRegistryClient SchemaRegistryClient { get; private set; }
        public Schema UnregisteredSchema { get; private set; }
        public RegisteredSchema RegisteredSchema { get; private set; }
        public JsonSerializerConfig SerializerConfig { get; private set; }
        public JsonDeserializerConfig DeserializerConfig { get; private set; }
        public JsonSchemaGeneratorSettings SchemaGeneratorSettings { get; private set; }

        public SchemaRegistryJsonSerializerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            SchemaRegistryClient = SchemaRegistryClientFactory.GetOrCreateSchemaRegistryClient(
                _serviceProvider,
                configureClient,
                clientKey);

            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaConfiguration(
            Action<ISchemaBuilder> configureSchema)
        {
            var builder = SchemaBuilder.Configure(configureSchema);

            UnregisteredSchema = builder.UnregisteredSchema;
            RegisteredSchema = builder.RegisteredSchema;

            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSerializerConfiguration(
            Action<IJsonSerializerConfigBuilder> configureSerializer)
        {
            SerializerConfig = JsonSerializerConfigBuilder.Build(configureSerializer);
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithDeserializerConfiguration(
            Action<IJsonDeserializerConfigBuilder> configureDeserializer)
        {
            DeserializerConfig = JsonDeserializerConfigBuilder.Build(configureDeserializer);
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorConfiguration(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator)
        {
            SchemaGeneratorSettings = JsonSchemaGeneratorSettingsBuilder.Build(configureSchemaGenerator);
            return this;
        }

        public static SchemaRegistryJsonSerializerBuilder Configure(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, ISchemaRegistryJsonSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryJsonSerializerBuilder(serviceProvider);

            configureSerializer?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
