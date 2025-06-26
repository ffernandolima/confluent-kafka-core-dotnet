using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
{
    public interface IAvroSerializerConfigBuilder
    {
        IAvroSerializerConfigBuilder FromConfiguration(string sectionKey);

        IAvroSerializerConfigBuilder WithBufferBytes(int? bufferBytes);

        IAvroSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas);

        IAvroSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas);

        IAvroSerializerConfigBuilder WithUseSchemaId(int? useSchemaId);

        IAvroSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IAvroSerializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata);

        IAvroSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);

        IAvroSerializerConfigBuilder WithSchemaIdStrategy(SchemaIdSerializerStrategy? SchemaIdStrategy);

        IAvroSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty);
    }
}
