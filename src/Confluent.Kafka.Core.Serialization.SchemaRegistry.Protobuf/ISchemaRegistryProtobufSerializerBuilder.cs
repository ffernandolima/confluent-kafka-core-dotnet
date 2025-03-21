﻿using Confluent.SchemaRegistry;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf
{
    public interface ISchemaRegistryProtobufSerializerBuilder
    {
        ISchemaRegistryProtobufSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryProtobufSerializerBuilder WithSerializerConfiguration(
            Action<IProtobufSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryProtobufSerializerBuilder WithDeserializerConfiguration(
            Action<IProtobufDeserializerConfigBuilder> configureDeserializer);

        ISchemaRegistryProtobufSerializerBuilder WithRuleRegistry(
            RuleRegistry ruleRegistry);
    }
}
