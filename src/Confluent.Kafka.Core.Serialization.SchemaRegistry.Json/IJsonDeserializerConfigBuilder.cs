using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface IJsonDeserializerConfigBuilder
    {
        IJsonDeserializerConfigBuilder FromConfiguration(string sectionKey);

        IJsonDeserializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IJsonDeserializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata);

        IJsonDeserializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IJsonDeserializerConfigBuilder WithSchemaIdStrategy(SchemaIdDeserializerStrategy? schemaIdStrategy);

        IJsonDeserializerConfigBuilder WithValidate(bool? validate);

        IJsonDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
