using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
{
    public interface IAvroDeserializerConfigBuilder
    {
        IAvroDeserializerConfigBuilder FromConfiguration(string sectionKey);

        IAvroDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
