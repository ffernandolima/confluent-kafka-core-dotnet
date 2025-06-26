using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class JsonDeserializerConfigBuilder :
        FunctionalBuilder<JsonDeserializerConfig, JsonDeserializerConfigBuilder>,
        IJsonDeserializerConfigBuilder
    {
        public JsonDeserializerConfigBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public IJsonDeserializerConfigBuilder FromConfiguration(string sectionKey)
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

        public IJsonDeserializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion)
        {
            AppendAction(config => config.UseLatestVersion = useLatestVersion);
            return this;
        }

        public IJsonDeserializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata)
        {
            AppendAction(config => config.UseLatestWithMetadata = useLatestWithMetadata);
            return this;
        }

        public IJsonDeserializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public IJsonDeserializerConfigBuilder WithSchemaIdStrategy(SchemaIdDeserializerStrategy? schemaIdStrategy)
        {
            AppendAction(config => config.SchemaIdStrategy = schemaIdStrategy);
            return this;
        }

        public IJsonDeserializerConfigBuilder WithValidate(bool? validate)
        {
            AppendAction(config => config.Validate = validate);
            return this;
        }

        public IJsonDeserializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static JsonDeserializerConfig Build(
            IConfiguration configuration,
            Action<IJsonDeserializerConfigBuilder> configureDeserializer)
        {
            using var builder = new JsonDeserializerConfigBuilder(configuration);

            configureDeserializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
