using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf
{
    public interface IProtobufSerializerConfigBuilder
    {
        IProtobufSerializerConfigBuilder WithBufferBytes(int? bufferBytes);

        IProtobufSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas);

        IProtobufSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas);

        IProtobufSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IProtobufSerializerConfigBuilder WithSkipKnownTypes(bool? skipKnownTypes);

        IProtobufSerializerConfigBuilder WithUseDeprecatedFormat(bool? useDeprecatedFormat);

        IProtobufSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IProtobufSerializerConfigBuilder WithReferenceSubjectNameStrategy(ReferenceSubjectNameStrategy? referenceSubjectNameStrategy);

        IProtobufSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
