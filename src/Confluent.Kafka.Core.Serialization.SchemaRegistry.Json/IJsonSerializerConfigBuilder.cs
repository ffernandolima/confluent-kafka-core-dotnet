using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface IJsonSerializerConfigBuilder
    {
        IJsonSerializerConfigBuilder WithBufferBytes(int? bufferBytes);

        IJsonSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas);

        IJsonSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas);

        IJsonSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IJsonSerializerConfigBuilder WithLatestCompatibilityStrict(bool? latestCompatibilityStrict);

        IJsonSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IJsonSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
