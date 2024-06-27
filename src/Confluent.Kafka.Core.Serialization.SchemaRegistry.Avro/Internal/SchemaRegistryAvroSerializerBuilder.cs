using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class SchemaRegistryAvroSerializerBuilder : ISchemaRegistryAvroSerializerBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ISchemaRegistryClient SchemaRegistryClient { get; private set; }
        public AvroSerializerConfig SerializerConfig { get; private set; }
        public AvroDeserializerConfig DeserializerConfig { get; private set; }

        public SchemaRegistryAvroSerializerBuilder(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public ISchemaRegistryAvroSerializerBuilder WithSchemaRegistryClient(
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

        public ISchemaRegistryAvroSerializerBuilder WithSerializerConfiguration(
            Action<IAvroSerializerConfigBuilder> configureSerializer)
        {
            SerializerConfig = AvroSerializerConfigBuilder.Build(_configuration, configureSerializer);
            return this;
        }

        public ISchemaRegistryAvroSerializerBuilder WithDeserializerConfiguration(
            Action<IAvroDeserializerConfigBuilder> configureDeserializer)
        {
            DeserializerConfig = AvroDeserializerConfigBuilder.Build(_configuration, configureDeserializer);
            return this;
        }

        public static SchemaRegistryAvroSerializerBuilder Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryAvroSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryAvroSerializerBuilder(serviceProvider, configuration);

            configureSerializer?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
