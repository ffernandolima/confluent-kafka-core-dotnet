using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf
{
    public interface IProtobufDeserializerConfigBuilder
    {
        IProtobufDeserializerConfigBuilder WithUseDeprecatedFormat(bool? useDeprecatedFormat);

        IProtobufDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
