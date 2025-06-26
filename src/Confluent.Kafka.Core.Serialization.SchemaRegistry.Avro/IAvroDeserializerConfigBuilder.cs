using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
{
    public interface IAvroDeserializerConfigBuilder
    {
        IAvroDeserializerConfigBuilder FromConfiguration(string sectionKey);

        IAvroDeserializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IAvroDeserializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata);

        IAvroDeserializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IAvroDeserializerConfigBuilder WithSchemaIdStrategy(SchemaIdDeserializerStrategy? SchemaIdStrategy);

        IAvroDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
