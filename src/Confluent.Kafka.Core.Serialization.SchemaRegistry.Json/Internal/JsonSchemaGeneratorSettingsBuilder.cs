using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using Namotion.Reflection;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class JsonSchemaGeneratorSettingsBuilder :
        FunctionalBuilder<JsonSchemaGeneratorSettings, JsonSchemaGeneratorSettingsBuilder>,
        IJsonSchemaGeneratorSettingsBuilder
    {
        public JsonSchemaGeneratorSettingsBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        protected override JsonSchemaGeneratorSettings CreateSubject() => new()
        {
            FlattenInheritanceHierarchy = true
        };

        public IJsonSchemaGeneratorSettingsBuilder FromConfiguration(string sectionKey)
        {
            AppendAction(settings =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    settings = Bind(settings, sectionKey);
                }
            });
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithDefaultReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultReferenceTypeNullHandling)
        {
            AppendAction(settings => settings.DefaultReferenceTypeNullHandling = defaultReferenceTypeNullHandling);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithDefaultDictionaryValueReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultDictionaryValueReferenceTypeNullHandling)
        {
            AppendAction(settings =>
            {
                settings.DefaultDictionaryValueReferenceTypeNullHandling = defaultDictionaryValueReferenceTypeNullHandling;
            });
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithGenerateAbstractProperties(bool generateAbstractProperties)
        {
            AppendAction(settings => settings.GenerateAbstractProperties = generateAbstractProperties);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithFlattenInheritanceHierarchy(bool flattenInheritanceHierarchy)
        {
            AppendAction(settings => settings.FlattenInheritanceHierarchy = flattenInheritanceHierarchy);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithGenerateAbstractSchemas(bool generateAbstractSchemas)
        {
            AppendAction(settings => settings.GenerateAbstractSchemas = generateAbstractSchemas);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithGenerateKnownTypes(bool generateKnownTypes)
        {
            AppendAction(settings => settings.GenerateKnownTypes = generateKnownTypes);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithGenerateXmlObjects(bool generateXmlObjects)
        {
            AppendAction(settings => settings.GenerateXmlObjects = generateXmlObjects);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithIgnoreObsoleteProperties(bool ignoreObsoleteProperties)
        {
            AppendAction(settings => settings.IgnoreObsoleteProperties = ignoreObsoleteProperties);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithAllowReferencesWithProperties(bool allowReferencesWithProperties)
        {
            AppendAction(settings => settings.AllowReferencesWithProperties = allowReferencesWithProperties);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithGenerateEnumMappingDescription(bool generateEnumMappingDescription)
        {
            AppendAction(settings => settings.GenerateEnumMappingDescription = generateEnumMappingDescription);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithAlwaysAllowAdditionalObjectProperties(bool alwaysAllowAdditionalObjectProperties)
        {
            AppendAction(settings => settings.AlwaysAllowAdditionalObjectProperties = alwaysAllowAdditionalObjectProperties);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithGenerateExamples(bool generateExamples)
        {
            AppendAction(settings => settings.GenerateExamples = generateExamples);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithSchemaType(SchemaType schemaType)
        {
            AppendAction(settings => settings.SchemaType = schemaType);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            AppendAction(settings => settings.SerializerSettings = serializerSettings);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithSerializerOptions(object serializerOptions)
        {
            AppendAction(settings => settings.SerializerOptions = serializerOptions);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithExcludedTypeNames(string[] excludedTypeNames)
        {
            AppendAction(settings => settings.ExcludedTypeNames = excludedTypeNames);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithUseXmlDocumentation(bool useXmlDocumentation)
        {
            AppendAction(settings => settings.UseXmlDocumentation = useXmlDocumentation);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithResolveExternalXmlDocumentation(bool resolveExternalXmlDocumentation)
        {
            AppendAction(settings => settings.ResolveExternalXmlDocumentation = resolveExternalXmlDocumentation);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithXmlDocumentationFormatting(XmlDocsFormattingMode xmlDocumentationFormatting)
        {
            AppendAction(settings => settings.XmlDocumentationFormatting = xmlDocumentationFormatting);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithTypeNameGenerator(ITypeNameGenerator typeNameGenerator)
        {
            AppendAction(settings => settings.TypeNameGenerator = typeNameGenerator);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithSchemaNameGenerator(ISchemaNameGenerator schemaNameGenerator)
        {
            AppendAction(settings => settings.SchemaNameGenerator = schemaNameGenerator);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithReflectionService(IReflectionService reflectionService)
        {
            AppendAction(settings => settings.ReflectionService = reflectionService);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithTypeMappers(ICollection<ITypeMapper> typeMappers)
        {
            AppendAction(settings => settings.TypeMappers = typeMappers);
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithSchemaProcessors(ICollection<ISchemaProcessor> schemaProcessors)
        {
            AppendAction(settings =>
            {
                if (settings.SchemaProcessors is not null &&
                             schemaProcessors is not null &&
                             schemaProcessors.Any(schemaProcessor => schemaProcessor is not null))
                {
                    foreach (var schemaProcessor in schemaProcessors.Where(schemaProcessor => schemaProcessor is not null))
                    {
                        if (!settings.SchemaProcessors.Contains(schemaProcessor))
                        {
                            settings.SchemaProcessors.Add(schemaProcessor);
                        }
                    }
                }
            });
            return this;
        }

        public IJsonSchemaGeneratorSettingsBuilder WithGenerateCustomNullableProperties(bool generateCustomNullableProperties)
        {
            AppendAction(settings => settings.GenerateCustomNullableProperties = generateCustomNullableProperties);
            return this;
        }

        public static JsonSchemaGeneratorSettings Build(
            IConfiguration configuration,
            Action<IJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator)
        {
            using var builder = new JsonSchemaGeneratorSettingsBuilder(configuration);

            configureSchemaGenerator?.Invoke(builder);

            var settings = builder.Build();

            return settings;
        }
    }
}
