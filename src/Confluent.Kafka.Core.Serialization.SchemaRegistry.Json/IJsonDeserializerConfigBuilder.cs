using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface IJsonDeserializerConfigBuilder
    {
        IJsonDeserializerConfigBuilder FromConfiguration(string sectionKey);

        IJsonDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
