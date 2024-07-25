using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class AvroDeserializerConfigBuilder :
        FunctionalBuilder<AvroDeserializerConfig, AvroDeserializerConfigBuilder>,
        IAvroDeserializerConfigBuilder
    {
        public AvroDeserializerConfigBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public IAvroDeserializerConfigBuilder FromConfiguration(string sectionKey)
        {
            AppendAction(config =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    config = Bind(config, sectionKey);
                }
            });
            return this;
        }

        public IAvroDeserializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion)
        {
            AppendAction(config => config.UseLatestVersion = useLatestVersion);
            return this;
        }

        public IAvroDeserializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata)
        {
            AppendAction(config => config.UseLatestWithMetadata = useLatestWithMetadata);
            return this;
        }

        public IAvroDeserializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public IAvroDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static AvroDeserializerConfig Build(
            IConfiguration configuration,
            Action<IAvroDeserializerConfigBuilder> configureDeserializer)
        {
            using var builder = new AvroDeserializerConfigBuilder(configuration);

            configureDeserializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
