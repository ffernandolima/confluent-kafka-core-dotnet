using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface IJsonSerializerConfigBuilder
    {
        IJsonSerializerConfigBuilder FromConfiguration(string sectionKey);

        IJsonSerializerConfigBuilder WithBufferBytes(int? bufferBytes);

        IJsonSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas);

        IJsonSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas);

        IJsonSerializerConfigBuilder WithUseSchemaId(int? useSchemaId);

        IJsonSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IJsonSerializerConfigBuilder WithLatestCompatibilityStrict(bool? latestCompatibilityStrict);

        IJsonSerializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata);

        IJsonSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IJsonSerializerConfigBuilder WithSchemaIdStrategy(SchemaIdSerializerStrategy? schemaIdStrategy);

        IJsonSerializerConfigBuilder WithValidate(bool? validate);

        IJsonSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
