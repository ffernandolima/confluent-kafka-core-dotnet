using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class JsonDeserializerConfigBuilder :
        FunctionalBuilder<JsonDeserializerConfig, JsonDeserializerConfigBuilder>,
        IJsonDeserializerConfigBuilder
    {
        public IJsonDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static JsonDeserializerConfig Build(Action<IJsonDeserializerConfigBuilder> configureDeserializer)
        {
            using var builder = new JsonDeserializerConfigBuilder();

            configureDeserializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
