using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf
{
    public interface IProtobufDeserializerConfigBuilder
    {
        IProtobufDeserializerConfigBuilder FromConfiguration(string sectionKey);

        IProtobufDeserializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IProtobufDeserializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata);

        IProtobufDeserializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IProtobufDeserializerConfigBuilder WithUseDeprecatedFormat(bool? useDeprecatedFormat);

        IProtobufDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
