using Confluent.SchemaRegistry;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
{
    public interface IAvroSerializerConfigBuilder
    {
        IAvroSerializerConfigBuilder FromConfiguration(string sectionKey);

        IAvroSerializerConfigBuilder WithBufferBytes(int? bufferBytes);

        IAvroSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas);

        IAvroSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas);

        IAvroSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IAvroSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);
    }
}
