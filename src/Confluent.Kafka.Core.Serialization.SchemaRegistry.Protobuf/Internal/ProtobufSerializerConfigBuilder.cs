using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class ProtobufSerializerConfigBuilder :
        FunctionalBuilder<ProtobufSerializerConfig, ProtobufSerializerConfigBuilder>,
        IProtobufSerializerConfigBuilder
    {
        public ProtobufSerializerConfigBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        protected override ProtobufSerializerConfig CreateSubject() => new()
        {
            AutoRegisterSchemas = true
        };

        public IProtobufSerializerConfigBuilder FromConfiguration(string sectionKey)
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

        public IProtobufSerializerConfigBuilder WithBufferBytes(int? bufferBytes)
        {
            AppendAction(config => config.BufferBytes = bufferBytes);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas)
        {
            AppendAction(config => config.AutoRegisterSchemas = autoRegisterSchemas);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas)
        {
            AppendAction(config => config.NormalizeSchemas = normalizeSchemas);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion)
        {
            AppendAction(config => config.UseLatestVersion = useLatestVersion);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata)
        {
            AppendAction(config => config.UseLatestWithMetadata = useLatestWithMetadata);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithSkipKnownTypes(bool? skipKnownTypes)
        {
            AppendAction(config => config.SkipKnownTypes = skipKnownTypes);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithUseDeprecatedFormat(bool? useDeprecatedFormat)
        {
            AppendAction(config => config.UseDeprecatedFormat = useDeprecatedFormat);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithReferenceSubjectNameStrategy(ReferenceSubjectNameStrategy? referenceSubjectNameStrategy)
        {
            AppendAction(config => config.ReferenceSubjectNameStrategy = referenceSubjectNameStrategy);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithCustomReferenceSubjectNameStrategy(ICustomReferenceSubjectNameStrategy customReferenceSubjectNameStrategy)
        {
            AppendAction(config => config.CustomReferenceSubjectNameStrategy = customReferenceSubjectNameStrategy);
            return this;
        }

        public IProtobufSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static ProtobufSerializerConfig Build(
            IConfiguration configuration,
            Action<IProtobufSerializerConfigBuilder> configureSerializer)
        {
            using var builder = new ProtobufSerializerConfigBuilder(configuration);

            configureSerializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
