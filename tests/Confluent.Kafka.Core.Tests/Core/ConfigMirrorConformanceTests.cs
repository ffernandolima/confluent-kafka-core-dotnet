using Confluent.Kafka.Core.Client;
using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Confluent.Kafka.Core.Tests.Conformance;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core
{
    /// <summary>
    /// The config mirror is hand-maintained and not enforced by the compiler, so a Confluent upgrade
    /// can leave a new upstream property unreachable through the fluent API. These tests catch that.
    /// </summary>
    public sealed class ConfigMirrorConformanceTests
    {
        [Fact]
        public void ClientConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(ClientConfig), typeof(IClientConfig), typeof(IKafkaConsumerConfigBuilder));
            AssertMirrored(typeof(ClientConfig), typeof(IClientConfig), typeof(IKafkaProducerConfigBuilder));
        }

        [Fact]
        public void ConsumerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(ConsumerConfig), typeof(IConsumerConfig), typeof(IKafkaConsumerConfigBuilder));
        }

        [Fact]
        public void ProducerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(ProducerConfig), typeof(IProducerConfig), typeof(IKafkaProducerConfigBuilder));
        }

        [Fact]
        public void SchemaRegistryConfig_IsFullyMirrored()
        {
            // ISchemaRegistryConfigBuilder is compiled into the Avro, Json and Protobuf assemblies from
            // src/Shared, so it cannot be named directly from here without an ambiguous reference.
            var schemaRegistryConfigBuilderType = typeof(IJsonSerializerConfigBuilder)
                .Assembly
                .GetType("Confluent.Kafka.Core.Serialization.SchemaRegistry.ISchemaRegistryConfigBuilder", throwOnError: true);

            AssertMirrored(typeof(SchemaRegistryConfig), mirrorInterface: null, schemaRegistryConfigBuilderType);
        }

        [Fact]
        public void AvroSerializerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(AvroSerializerConfig), mirrorInterface: null, typeof(IAvroSerializerConfigBuilder));
        }

        [Fact]
        public void AvroDeserializerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(AvroDeserializerConfig), mirrorInterface: null, typeof(IAvroDeserializerConfigBuilder));
        }

        [Fact]
        public void JsonSerializerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(JsonSerializerConfig), mirrorInterface: null, typeof(IJsonSerializerConfigBuilder));
        }

        [Fact]
        public void JsonDeserializerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(JsonDeserializerConfig), mirrorInterface: null, typeof(IJsonDeserializerConfigBuilder));
        }

        [Fact]
        public void ProtobufSerializerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(ProtobufSerializerConfig), mirrorInterface: null, typeof(IProtobufSerializerConfigBuilder));
        }

        [Fact]
        public void ProtobufDeserializerConfig_IsFullyMirrored()
        {
            AssertMirrored(typeof(ProtobufDeserializerConfig), mirrorInterface: null, typeof(IProtobufDeserializerConfigBuilder));
        }

        private static void AssertMirrored(Type confluentConfigType, Type mirrorInterface, Type builderInterface)
        {
            MirrorConformance.AssertMirrored(confluentConfigType, builderInterface, mirrorInterface);
        }
    }
}
