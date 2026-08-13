using Confluent.Kafka.Core.Client;
using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core
{
    /// <summary>
    /// Confluent.Kafka.Core hand-mirrors the Confluent config surface across several hundred members.
    /// Nothing in the compiler enforces that mirror, so a Confluent package upgrade can silently leave
    /// a newly added upstream config property unreachable through this library's fluent API.
    /// These tests fail when that drift happens.
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
                .GetType("Confluent.Kafka.Core.Serialization.SchemaRegistry.ISchemaRegistryConfigBuilder", throwOnError: true)

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
            var properties = GetMirrorableProperties(confluentConfigType);

            // Guards against the conformance check silently becoming vacuous if Confluent moves these properties onto a base type.
            Assert.True(
                properties.Count > 0,
                $"No mirrorable properties were discovered on {confluentConfigType.FullName}. " +
                "The declaring type has probably changed upstream and this test needs to be widened.");

            var missing = new List<string>();

            foreach (var property in properties)
            {
                if (mirrorInterface is not null && !HasProperty(mirrorInterface, property.Name))
                {
                    missing.Add($"{mirrorInterface.Name}.{property.Name}");
                }

                if (!HasMethod(builderInterface, $"With{property.Name}"))
                {
                    missing.Add($"{builderInterface.Name}.With{property.Name}");
                }
            }

            Assert.True(
                missing.Count == 0,
                $"{confluentConfigType.FullName} exposes members that Confluent.Kafka.Core does not mirror, " +
                "which usually means the Confluent packages were upgraded without syncing the mirrors. " +
                $"Missing: {string.Join(", ", missing)}");
        }

        private static IReadOnlyList<PropertyInfo> GetMirrorableProperties(Type configType)
        {
            return [.. configType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(property => property.CanRead && property.CanWrite)
                .Where(property => property.GetIndexParameters().Length == 0)
                .Where(property => property.GetCustomAttribute<ObsoleteAttribute>() is null)
                .OrderBy(property => property.Name, StringComparer.Ordinal)];
        }

        private static bool HasProperty(Type interfaceType, string name)
        {
            return WithInheritedInterfaces(interfaceType).Any(type => type.GetProperty(name) is not null);
        }

        private static bool HasMethod(Type interfaceType, string name)
        {
            return WithInheritedInterfaces(interfaceType)
                .SelectMany(type => type.GetMethods())
                .Any(method => string.Equals(method.Name, name, StringComparison.Ordinal));
        }

        // Interface reflection does not surface inherited members, so the base interfaces are walked explicitly.
        private static IEnumerable<Type> WithInheritedInterfaces(Type interfaceType)
        {
            return new[] { interfaceType }.Concat(interfaceType.GetInterfaces());
        }
    }
}
