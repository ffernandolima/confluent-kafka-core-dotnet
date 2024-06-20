using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class SchemaRegistryAvroSerializerBuilder : ISchemaRegistryAvroSerializerBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public ISchemaRegistryClient SchemaRegistryClient { get; private set; }
        public AvroSerializerConfig SerializerConfig { get; private set; }
        public AvroDeserializerConfig DeserializerConfig { get; private set; }

        public SchemaRegistryAvroSerializerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISchemaRegistryAvroSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            SchemaRegistryClient = SchemaRegistryClientFactory.GetOrCreateSchemaRegistryClient(
                _serviceProvider,
                configureClient,
                clientKey);

            return this;
        }

        public ISchemaRegistryAvroSerializerBuilder WithSerializerConfiguration(
            Action<IAvroSerializerConfigBuilder> configureSerializer)
        {
            SerializerConfig = AvroSerializerConfigBuilder.Build(configureSerializer);
            return this;
        }

        public ISchemaRegistryAvroSerializerBuilder WithDeserializerConfiguration(
            Action<IAvroDeserializerConfigBuilder> configureDeserializer)
        {
            DeserializerConfig = AvroDeserializerConfigBuilder.Build(configureDeserializer);
            return this;
        }

        public static SchemaRegistryAvroSerializerBuilder Configure(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, ISchemaRegistryAvroSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryAvroSerializerBuilder(serviceProvider);

            configureSerializer?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
