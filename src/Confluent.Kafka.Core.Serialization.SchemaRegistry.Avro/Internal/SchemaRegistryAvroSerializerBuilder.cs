using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class SchemaRegistryAvroSerializerBuilder : ISchemaRegistryAvroSerializerBuilder
    {
        public object ClientKey { get; private set; }
        public Action<ISchemaRegistryClientBuilder> ConfigureClient { get; private set; }
        public Action<IAvroSerializerConfigBuilder> ConfigureSerializer { get; private set; }
        public Action<IAvroDeserializerConfigBuilder> ConfigureDeserializer { get; private set; }

        public ISchemaRegistryAvroSerializerBuilder WithSchemaRegistryClientConfiguration(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            ConfigureClient = configureClient;
            ClientKey = clientKey;
            return this;
        }

        public ISchemaRegistryAvroSerializerBuilder WithAvroSerializerConfiguration(
            Action<IAvroSerializerConfigBuilder> configureSerializer)
        {
            ConfigureSerializer = configureSerializer;
            return this;
        }

        public ISchemaRegistryAvroSerializerBuilder WithAvroDeserializerConfiguration(
            Action<IAvroDeserializerConfigBuilder> configureDeserializer)
        {
            ConfigureDeserializer = configureDeserializer;
            return this;
        }

        public static SchemaRegistryAvroSerializerBuilder Configure(
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryAvroSerializerBuilder();

            configureSerializer?.Invoke(builder);

            return builder;
        }
    }
}
