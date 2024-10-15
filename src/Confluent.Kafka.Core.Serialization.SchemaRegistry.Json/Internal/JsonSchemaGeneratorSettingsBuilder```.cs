using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal abstract class JsonSchemaGeneratorSettingsBuilder<TSettings, TService, TImplementation> :
        FunctionalBuilder<TSettings, TImplementation>
            where TSettings : JsonSchemaGeneratorSettings
            where TService : IJsonSchemaGeneratorSettingsBuilder<TService>
            where TImplementation : JsonSchemaGeneratorSettingsBuilder<TSettings, TService, TImplementation>, TService
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract TService BuilderInstance { get; }

        public JsonSchemaGeneratorSettingsBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public TService FromConfiguration(string sectionKey)
        {
            AppendAction(settings =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    settings = Bind(settings, sectionKey);
                }
            });
            return BuilderInstance;
        }

        public TService WithDefaultReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultReferenceTypeNullHandling)
        {
            AppendAction(settings => settings.DefaultReferenceTypeNullHandling = defaultReferenceTypeNullHandling);
            return BuilderInstance;
        }

        public TService WithDefaultDictionaryValueReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultDictionaryValueReferenceTypeNullHandling)
        {
            AppendAction(settings =>
            {
                settings.DefaultDictionaryValueReferenceTypeNullHandling = defaultDictionaryValueReferenceTypeNullHandling;
            });
            return BuilderInstance;
        }

        public TService WithGenerateAbstractProperties(bool generateAbstractProperties)
        {
            AppendAction(settings => settings.GenerateAbstractProperties = generateAbstractProperties);
            return BuilderInstance;
        }

        public TService WithFlattenInheritanceHierarchy(bool flattenInheritanceHierarchy)
        {
            AppendAction(settings => settings.FlattenInheritanceHierarchy = flattenInheritanceHierarchy);
            return BuilderInstance;
        }

        public TService WithGenerateAbstractSchemas(bool generateAbstractSchemas)
        {
            AppendAction(settings => settings.GenerateAbstractSchemas = generateAbstractSchemas);
            return BuilderInstance;
        }

        public TService WithGenerateKnownTypes(bool generateKnownTypes)
        {
            AppendAction(settings => settings.GenerateKnownTypes = generateKnownTypes);
            return BuilderInstance;
        }

        public TService WithGenerateXmlObjects(bool generateXmlObjects)
        {
            AppendAction(settings => settings.GenerateXmlObjects = generateXmlObjects);
            return BuilderInstance;
        }

        public TService WithIgnoreObsoleteProperties(bool ignoreObsoleteProperties)
        {
            AppendAction(settings => settings.IgnoreObsoleteProperties = ignoreObsoleteProperties);
            return BuilderInstance;
        }

        public TService WithAllowReferencesWithProperties(bool allowReferencesWithProperties)
        {
            AppendAction(settings => settings.AllowReferencesWithProperties = allowReferencesWithProperties);
            return BuilderInstance;
        }

        public TService WithGenerateEnumMappingDescription(bool generateEnumMappingDescription)
        {
            AppendAction(settings => settings.GenerateEnumMappingDescription = generateEnumMappingDescription);
            return BuilderInstance;
        }

        public TService WithAlwaysAllowAdditionalObjectProperties(bool alwaysAllowAdditionalObjectProperties)
        {
            AppendAction(settings => settings.AlwaysAllowAdditionalObjectProperties = alwaysAllowAdditionalObjectProperties);
            return BuilderInstance;
        }

        public TService WithGenerateExamples(bool generateExamples)
        {
            AppendAction(settings => settings.GenerateExamples = generateExamples);
            return BuilderInstance;
        }

        public TService WithSchemaType(SchemaType schemaType)
        {
            AppendAction(settings => settings.SchemaType = schemaType);
            return BuilderInstance;
        }

        public TService WithExcludedTypeNames(string[] excludedTypeNames)
        {
            AppendAction(settings => settings.ExcludedTypeNames = excludedTypeNames);
            return BuilderInstance;
        }

        public TService WithUseXmlDocumentation(bool useXmlDocumentation)
        {
            AppendAction(settings => settings.UseXmlDocumentation = useXmlDocumentation);
            return BuilderInstance;
        }

        public TService WithResolveExternalXmlDocumentation(bool resolveExternalXmlDocumentation)
        {
            AppendAction(settings => settings.ResolveExternalXmlDocumentation = resolveExternalXmlDocumentation);
            return BuilderInstance;
        }

        public TService WithXmlDocumentationFormatting(XmlDocsFormattingMode xmlDocumentationFormatting)
        {
            AppendAction(settings => settings.XmlDocumentationFormatting = xmlDocumentationFormatting);
            return BuilderInstance;
        }

        public TService WithTypeNameGenerator(ITypeNameGenerator typeNameGenerator)
        {
            AppendAction(settings => settings.TypeNameGenerator = typeNameGenerator);
            return BuilderInstance;
        }

        public TService WithSchemaNameGenerator(ISchemaNameGenerator schemaNameGenerator)
        {
            AppendAction(settings => settings.SchemaNameGenerator = schemaNameGenerator);
            return BuilderInstance;
        }

        public TService WithReflectionService(IReflectionService reflectionService)
        {
            AppendAction(settings => settings.ReflectionService = reflectionService);
            return BuilderInstance;
        }

        public TService WithTypeMappers(ICollection<ITypeMapper> typeMappers)
        {
            AppendAction(settings => settings.TypeMappers = typeMappers);
            return BuilderInstance;
        }

        public TService WithSchemaProcessors(ICollection<ISchemaProcessor> schemaProcessors)
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
            return BuilderInstance;
        }

        public TService WithGenerateCustomNullableProperties(bool generateCustomNullableProperties)
        {
            AppendAction(settings => settings.GenerateCustomNullableProperties = generateCustomNullableProperties);
            return BuilderInstance;
        }
    }
}
