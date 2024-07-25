using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class ProtobufDeserializerConfigBuilder :
        FunctionalBuilder<ProtobufDeserializerConfig, ProtobufDeserializerConfigBuilder>,
        IProtobufDeserializerConfigBuilder
    {
        public ProtobufDeserializerConfigBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public IProtobufDeserializerConfigBuilder FromConfiguration(string sectionKey)
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

        public IProtobufDeserializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion)
        {
            AppendAction(config => config.UseLatestVersion = useLatestVersion);
            return this;
        }

        public IProtobufDeserializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata)
        {
            AppendAction(config => config.UseLatestWithMetadata = useLatestWithMetadata);
            return this;
        }

        public IProtobufDeserializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public IProtobufDeserializerConfigBuilder WithUseDeprecatedFormat(bool? useDeprecatedFormat)
        {
            AppendAction(config => config.UseDeprecatedFormat = useDeprecatedFormat);
            return this;
        }

        public IProtobufDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static ProtobufDeserializerConfig Build(
            IConfiguration configuration,
            Action<IProtobufDeserializerConfigBuilder> configureDeserializer)
        {
            using var builder = new ProtobufDeserializerConfigBuilder(configuration);

            configureDeserializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
