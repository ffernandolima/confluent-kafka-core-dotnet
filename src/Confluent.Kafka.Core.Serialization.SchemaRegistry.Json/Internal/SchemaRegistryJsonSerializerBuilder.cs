using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
#if NET8_0_OR_GREATER
using NJsonSchema.NewtonsoftJson.Generation;
#else
using NJsonSchema.Generation;
#endif
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
#if NET8_0_OR_GREATER
        public NewtonsoftJsonSchemaGeneratorSettings SchemaGeneratorSettings { get; private set; }
#else
        public JsonSchemaGeneratorSettings SchemaGeneratorSettings { get; private set; }
#endif
        public RuleRegistry RuleRegistry { get; private set; }

        public SchemaRegistryJsonSerializerBuilder(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public ISchemaRegistryJsonSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            SchemaRegistryClient = SchemaRegistryClientFactory.Instance.GetOrCreateSchemaRegistryClient(
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

#if NET8_0_OR_GREATER
        public ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorSettings(
            Action<INewtonsoftJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator)
        {
            SchemaGeneratorSettings = NewtonsoftJsonSchemaGeneratorSettingsBuilder.Build(
                _configuration,
                configureSchemaGenerator);

            return this;
        }
#else
        public ISchemaRegistryJsonSerializerBuilder WithSchemaGeneratorSettings(
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator)
        {
            SchemaGeneratorSettings = JsonSchemaGeneratorSettingsBuilder.Build(
                _configuration,
                configureSchemaGenerator);

            return this;
        }
#endif
        public ISchemaRegistryJsonSerializerBuilder WithRuleRegistry(
            RuleRegistry ruleRegistry)
        {
            RuleRegistry = ruleRegistry;
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
