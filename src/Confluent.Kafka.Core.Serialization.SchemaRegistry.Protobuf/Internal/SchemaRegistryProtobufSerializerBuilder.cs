using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class SchemaRegistryProtobufSerializerBuilder : ISchemaRegistryProtobufSerializerBuilder
    {
        public object ClientKey { get; private set; }
        public Action<ISchemaRegistryClientBuilder> ConfigureClient { get; private set; }
        public Action<IProtobufSerializerConfigBuilder> ConfigureSerializer { get; private set; }
        public Action<IProtobufDeserializerConfigBuilder> ConfigureDeserializer { get; private set; }

        public ISchemaRegistryProtobufSerializerBuilder WithSchemaRegistryClientConfiguration(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            ConfigureClient = configureClient;
            ClientKey = clientKey;
            return this;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithProtobufSerializerConfiguration(
            Action<IProtobufSerializerConfigBuilder> configureSerializer)
        {
            ConfigureSerializer = configureSerializer;
            return this;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithProtobufDeserializerConfiguration(
            Action<IProtobufDeserializerConfigBuilder> configureDeserializer)
        {
            ConfigureDeserializer = configureDeserializer;
            return this;
        }

        public static SchemaRegistryProtobufSerializerBuilder Configure(
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryProtobufSerializerBuilder();

            configureSerializer?.Invoke(builder);

            return builder;
        }
    }
}
