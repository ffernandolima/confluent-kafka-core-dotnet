using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using NJsonSchema.Generation;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class SchemaRegistryJsonSerializerBuilder : ISchemaRegistryJsonSerializerBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ISchemaRegistryClient SchemaRegistryClient { get; private set; }
        public Schema UnregisteredSchema { get; private set; }
        public RegisteredSchema RegisteredSchema { get; private set; }
        public JsonSerializerConfig SerializerConfig { get; private set; }
        public JsonDeserializerConfig DeserializerConfig { get; private set; }
        public JsonSchemaGeneratorSettings SchemaGeneratorSettings { get; private set; }

        public SchemaRegistryJsonSerializerBuilder(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            SchemaRegistryClient = SchemaRegistryClientFactory.GetOrCreateSchemaRegistryClient(
                _serviceProvider,
                _configuration,
                configureClient,
                clientKey);

            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchema(
            Action<ISchemaBuilder> configureSchema)
        {
            var builder = SchemaBuilder.Configure(_configuration, configureSchema);

            UnregisteredSchema = builder.UnregisteredSchema;
            RegisteredSchema = builder.RegisteredSchema;

            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSerializerConfiguration(
            Action<IJsonSerializerConfigBuilder> configureSerializer)
        {
            SerializerConfig = JsonSerializerConfigBuilder.Build(_configuration, configureSerializer);
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithDeserializerConfiguration(
            Action<IJsonDeserializerConfigBuilder> configureDeserializer)
        {
            DeserializerConfig = JsonDeserializerConfigBuilder.Build(_configuration, configureDeserializer);
            return this;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorSettings(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator)
        {
            SchemaGeneratorSettings = JsonSchemaGeneratorSettingsBuilder.Build(
                _configuration,
                configureSchemaGenerator);

            return this;
        }

        public static SchemaRegistryJsonSerializerBuilder Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryJsonSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryJsonSerializerBuilder(serviceProvider, configuration);

            configureSerializer?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
