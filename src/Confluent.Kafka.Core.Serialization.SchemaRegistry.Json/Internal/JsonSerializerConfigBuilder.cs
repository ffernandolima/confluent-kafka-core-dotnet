using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class JsonSerializerConfigBuilder :
        FunctionalBuilder<JsonSerializerConfig, JsonSerializerConfigBuilder>,
        IJsonSerializerConfigBuilder
    {
        public JsonSerializerConfigBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        protected override JsonSerializerConfig CreateSubject() => new()
        {
            AutoRegisterSchemas = true
        };

        public IJsonSerializerConfigBuilder FromConfiguration(string sectionKey)
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

        public IJsonSerializerConfigBuilder WithBufferBytes(int? bufferBytes)
        {
            AppendAction(config => config.BufferBytes = bufferBytes);
            return this;
        }

        public IJsonSerializerConfigBuilder WithAutoRegisterSchemas(bool? autoRegisterSchemas)
        {
            AppendAction(config => config.AutoRegisterSchemas = autoRegisterSchemas);
            return this;
        }

        public IJsonSerializerConfigBuilder WithNormalizeSchemas(bool? normalizeSchemas)
        {
            AppendAction(config => config.NormalizeSchemas = normalizeSchemas);
            return this;
        }

        public IJsonSerializerConfigBuilder WithUseSchemaId(int? useSchemaId)
        {
            AppendAction(config => config.UseSchemaId = useSchemaId);
            return this;
        }

        public IJsonSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion)
        {
            AppendAction(config => config.UseLatestVersion = useLatestVersion);
            return this;
        }

        public IJsonSerializerConfigBuilder WithLatestCompatibilityStrict(bool? latestCompatibilityStrict)
        {
            AppendAction(config => config.LatestCompatibilityStrict = latestCompatibilityStrict);
            return this;
        }

        public IJsonSerializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata)
        {
            AppendAction(config => config.UseLatestWithMetadata = useLatestWithMetadata);
            return this;
        }

        public IJsonSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public IJsonSerializerConfigBuilder WithValidate(bool? validate)
        {
            AppendAction(config => config.Validate = validate);
            return this;
        }

        public IJsonSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static JsonSerializerConfig Build(
            IConfiguration configuration,
            Action<IJsonSerializerConfigBuilder> configureSerializer)
        {
            using var builder = new JsonSerializerConfigBuilder(configuration);

            configureSerializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
