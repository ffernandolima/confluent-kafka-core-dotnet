using Confluent.Kafka.Core.Client;
using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Mirrors
{
    /// <summary>
    /// The config mirror is hand-maintained and not enforced by the compiler, so a Confluent upgrade
    /// can leave a new upstream property unreachable through the fluent API. These tests catch that.
    /// </summary>
    public sealed class ConfigMirrorTests
    {
        [Fact]
        public void ClientConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(ClientConfig), typeof(IKafkaConsumerConfigBuilder), typeof(IClientConfig));
            MirrorAssert.IsComplete(typeof(ClientConfig), typeof(IKafkaProducerConfigBuilder), typeof(IClientConfig));
        }

        [Fact]
        public void ConsumerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(ConsumerConfig), typeof(IKafkaConsumerConfigBuilder), typeof(IConsumerConfig));
        }

        [Fact]
        public void ProducerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(ProducerConfig), typeof(IKafkaProducerConfigBuilder), typeof(IProducerConfig));
        }

        [Fact]
        public void SchemaRegistryConfig_IsFullyMirrored()
        {
            // ISchemaRegistryConfigBuilder is compiled into the Avro, Json and Protobuf assemblies from
            // src/Shared, so it cannot be named directly from here without an ambiguous reference.
            var schemaRegistryConfigBuilderType = typeof(IJsonSerializerConfigBuilder)
                .Assembly
                .GetType("Confluent.Kafka.Core.Serialization.SchemaRegistry.ISchemaRegistryConfigBuilder", throwOnError: true);

            MirrorAssert.IsComplete(typeof(SchemaRegistryConfig), schemaRegistryConfigBuilderType);
        }

        [Fact]
        public void AvroSerializerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(AvroSerializerConfig), typeof(IAvroSerializerConfigBuilder));
        }

        [Fact]
        public void AvroDeserializerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(AvroDeserializerConfig), typeof(IAvroDeserializerConfigBuilder));
        }

        [Fact]
        public void JsonSerializerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(JsonSerializerConfig), typeof(IJsonSerializerConfigBuilder));
        }

        [Fact]
        public void JsonDeserializerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(JsonDeserializerConfig), typeof(IJsonDeserializerConfigBuilder));
        }

        [Fact]
        public void ProtobufSerializerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(ProtobufSerializerConfig), typeof(IProtobufSerializerConfigBuilder));
        }

        [Fact]
        public void ProtobufDeserializerConfig_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(ProtobufDeserializerConfig), typeof(IProtobufDeserializerConfigBuilder));
        }
    }
}
