using Confluent.SchemaRegistry;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
{
    public interface ISchemaRegistryAvroSerializerBuilder
    {
        ISchemaRegistryAvroSerializerBuilder WithSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey = null);

        ISchemaRegistryAvroSerializerBuilder WithSerializerConfiguration(
            Action<IAvroSerializerConfigBuilder> configureSerializer);

        ISchemaRegistryAvroSerializerBuilder WithDeserializerConfiguration(
            Action<IAvroDeserializerConfigBuilder> configureDeserializer);

        ISchemaRegistryAvroSerializerBuilder WithRuleExecutors(
            IList<IRuleExecutor> ruleExecutors);
    }
}
