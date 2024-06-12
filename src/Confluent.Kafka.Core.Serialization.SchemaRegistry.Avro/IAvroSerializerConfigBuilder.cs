using Confluent.SchemaRegistry;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
{
    public interface IAvroSerializerConfigBuilder
    {
        IAvroSerializerConfigBuilder WithBufferBytes(int? bufferBytes);

        IAvroSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas);

        IAvroSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas);

        IAvroSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion);

        IAvroSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy);
    }
}
