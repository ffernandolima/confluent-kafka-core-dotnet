using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class AvroDeserializerConfigBuilder :
        FunctionalBuilder<AvroDeserializerConfig, AvroDeserializerConfigBuilder>,
        IAvroDeserializerConfigBuilder
    {
        public IAvroDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static AvroDeserializerConfig Build(Action<IAvroDeserializerConfigBuilder> configureDeserializer)
        {
            using var builder = new AvroDeserializerConfigBuilder();

            configureDeserializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
