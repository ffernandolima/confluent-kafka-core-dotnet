using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class AvroSerializerConfigBuilder :
        FunctionalBuilder<AvroSerializerConfig, AvroSerializerConfigBuilder>,
        IAvroSerializerConfigBuilder
    {
        protected override AvroSerializerConfig CreateSubject() => new()
        {
            AutoRegisterSchemas = true
        };

        public IAvroSerializerConfigBuilder WithBufferBytes(int? bufferBytes)
        {
            AppendAction(config => config.BufferBytes = bufferBytes);
            return this;
        }

        public IAvroSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas)
        {
            AppendAction(config => config.AutoRegisterSchemas = autoRegisterSchemas);
            return this;
        }

        public IAvroSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas)
        {
            AppendAction(config => config.NormalizeSchemas = normalizeSchemas);
            return this;
        }

        public IAvroSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion)
        {
            AppendAction(config => config.UseLatestVersion = useLatestVersion);
            return this;
        }

        public IAvroSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public static AvroSerializerConfig Build(Action<IAvroSerializerConfigBuilder> configureSerializer)
        {
            using var builder = new AvroSerializerConfigBuilder();

            configureSerializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
