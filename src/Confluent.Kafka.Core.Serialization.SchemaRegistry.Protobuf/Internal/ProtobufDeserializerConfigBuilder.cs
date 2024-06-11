using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class ProtobufDeserializerConfigBuilder :
        FunctionalBuilder<ProtobufDeserializerConfig, ProtobufDeserializerConfigBuilder>,
        IProtobufDeserializerConfigBuilder
    {
        public IProtobufDeserializerConfigBuilder WithUseDeprecatedFormat(bool? useDeprecatedFormat)
        {
            AppendAction(config => config.UseDeprecatedFormat = useDeprecatedFormat);
            return this;
        }

        public IProtobufDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static ProtobufDeserializerConfig Build(Action<IProtobufDeserializerConfigBuilder> configureDeserializer)
        {
            using var builder = new ProtobufDeserializerConfigBuilder();

            configureDeserializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
