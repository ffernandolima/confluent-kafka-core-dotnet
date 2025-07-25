﻿using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class AvroSerializerConfigBuilder :
        FunctionalBuilder<AvroSerializerConfig, AvroSerializerConfigBuilder>,
        IAvroSerializerConfigBuilder
    {
        public AvroSerializerConfigBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        protected override AvroSerializerConfig CreateSubject() => new()
        {
            AutoRegisterSchemas = true
        };

        public IAvroSerializerConfigBuilder FromConfiguration(string sectionKey)
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

        public IAvroSerializerConfigBuilder WithUseSchemaId(int? useSchemaId)
        {
            AppendAction(config => config.UseSchemaId = useSchemaId);
            return this;
        }

        public IAvroSerializerConfigBuilder WithUseLatestVersion(bool? useLatestVersion)
        {
            AppendAction(config => config.UseLatestVersion = useLatestVersion);
            return this;
        }

        public IAvroSerializerConfigBuilder WithUseLatestWithMetadata(IDictionary<string, string> useLatestWithMetadata)
        {
            AppendAction(config => config.UseLatestWithMetadata = useLatestWithMetadata);
            return this;
        }

        public IAvroSerializerConfigBuilder WithSubjectNameStrategy(SubjectNameStrategy? subjectNameStrategy)
        {
            AppendAction(config => config.SubjectNameStrategy = subjectNameStrategy);
            return this;
        }

        public IAvroSerializerConfigBuilder WithSchemaIdStrategy(SchemaIdSerializerStrategy? schemaIdStrategy)
        {
            AppendAction(config => config.SchemaIdStrategy = schemaIdStrategy);
            return this;
        }


        public IAvroSerializerConfigBuilder WithConfigurationProperty(KeyValuePair<string, string> configurationProperty)
        {
            AppendAction(config => config.Set(configurationProperty.Key, configurationProperty.Value));
            return this;
        }

        public static AvroSerializerConfig Build(
            IConfiguration configuration,
            Action<IAvroSerializerConfigBuilder> configureSerializer)
        {
            using var builder = new AvroSerializerConfigBuilder(configuration);

            configureSerializer?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
