using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class JsonSerializerConfigBuilder :
        FunctionalBuilder<JsonSerializerConfig, JsonSerializerConfigBuilder>,
        IJsonSerializerConfigBuilder
    {
        protected override JsonSerializerConfig CreateSubject() => new()
        {
            AutoRegisterSchemas = true
        };

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

        public IJsonSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public IJsonSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static JsonSerializerConfig Build(Action<IJsonSerializerConfigBuilder> configureSerializer)
        {
            using var builder = new JsonSerializerConfigBuilder();

            configureSerializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
