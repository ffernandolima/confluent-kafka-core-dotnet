using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class SchemaRegistryProtobufSerializerBuilder : ISchemaRegistryProtobufSerializerBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ISchemaRegistryClient SchemaRegistryClient { get; private set; }
        public ProtobufSerializerConfig SerializerConfig { get; private set; }
        public ProtobufDeserializerConfig DeserializerConfig { get; private set; }
        public IList<IRuleExecutor> RuleExecutors { get; private set; }

        public SchemaRegistryProtobufSerializerBuilder(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithSchemaRegistryClient(
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

        public ISchemaRegistryProtobufSerializerBuilder WithSerializerConfiguration(
            Action<IProtobufSerializerConfigBuilder> configureSerializer)
        {
            SerializerConfig = ProtobufSerializerConfigBuilder.Build(_configuration, configureSerializer);
            return this;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithDeserializerConfiguration(
            Action<IProtobufDeserializerConfigBuilder> configureDeserializer)
        {
            DeserializerConfig = ProtobufDeserializerConfigBuilder.Build(_configuration, configureDeserializer);
            return this;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithRuleExecutors(
            IList<IRuleExecutor> ruleExecutors)
        {
            RuleExecutors = ruleExecutors;
            return this;
        }

        public static SchemaRegistryProtobufSerializerBuilder Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryProtobufSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryProtobufSerializerBuilder(serviceProvider, configuration);

            configureSerializer?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
