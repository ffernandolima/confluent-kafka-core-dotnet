using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf
{
    public interface IProtobufSerializerConfigBuilder
    {
        IProtobufSerializerConfigBuilder FromConfiguration(string sectionKey);

        IProtobufSerializerConfigBuilder WithBufferBytes(int? bufferBytes);

        IProtobufSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas);

        IProtobufSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas);

        IProtobufSerializerConfigBuilder WithUseSchemaId(int? useSchemaId);

        IProtobufSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IProtobufSerializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata);

        IProtobufSerializerConfigBuilder WithSkipKnownTypes(bool? skipKnownTypes);

        IProtobufSerializerConfigBuilder WithUseDeprecatedFormat(bool? useDeprecatedFormat);

        IProtobufSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IProtobufSerializerConfigBuilder WithSchemaIdStrategy(SchemaIdSerializerStrategy? schemaIdStrategy);

        IProtobufSerializerConfigBuilder WithReferenceSubjectNameStrategy(ReferenceSubjectNameStrategy? referenceSubjectNameStrategy);

        IProtobufSerializerConfigBuilder WithCustomReferenceSubjectNameStrategy(ICustomReferenceSubjectNameStrategy customReferenceSubjectNameStrategy);

        IProtobufSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
