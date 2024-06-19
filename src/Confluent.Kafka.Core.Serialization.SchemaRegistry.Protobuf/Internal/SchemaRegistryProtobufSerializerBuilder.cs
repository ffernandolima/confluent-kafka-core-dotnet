using Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class SchemaRegistryProtobufSerializerBuilder : ISchemaRegistryProtobufSerializerBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public ISchemaRegistryClient SchemaRegistryClient { get; private set; }
        public ProtobufSerializerConfig SerializerConfig { get; private set; }
        public ProtobufDeserializerConfig DeserializerConfig { get; private set; }

        public SchemaRegistryProtobufSerializerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithSchemaRegistryClientConfiguration(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null)
        {
            SchemaRegistryClient = SchemaRegistryClientFactory.GetOrCreateSchemaRegistryClient(
                _serviceProvider,
                configureClient,
                clientKey);

            return this;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithProtobufSerializerConfiguration(
            Action<IProtobufSerializerConfigBuilder> configureSerializer)
        {
            SerializerConfig = ProtobufSerializerConfigBuilder.Build(configureSerializer);
            return this;
        }

        public ISchemaRegistryProtobufSerializerBuilder WithProtobufDeserializerConfiguration(
            Action<IProtobufDeserializerConfigBuilder> configureDeserializer)
        {
            DeserializerConfig = ProtobufDeserializerConfigBuilder.Build(configureDeserializer);
            return this;
        }

        public static SchemaRegistryProtobufSerializerBuilder Configure(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, ISchemaRegistryProtobufSerializerBuilder> configureSerializer)
        {
            var builder = new SchemaRegistryProtobufSerializerBuilder(serviceProvider);

            configureSerializer?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
