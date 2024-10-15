using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface IJsonSchemaGeneratorSettingsBuilder<TBuilder>
        where TBuilder : IJsonSchemaGeneratorSettingsBuilder<TBuilder>
    {
        TBuilder FromConfiguration(string sectionKey);

        TBuilder WithDefaultReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultReferenceTypeNullHandling);

        TBuilder WithDefaultDictionaryValueReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultDictionaryValueReferenceTypeNullHandling);

        TBuilder WithGenerateAbstractProperties(bool generateAbstractProperties);

        TBuilder WithFlattenInheritanceHierarchy(bool flattenInheritanceHierarchy);

        TBuilder WithGenerateAbstractSchemas(bool generateAbstractSchemas);

        TBuilder WithGenerateKnownTypes(bool generateKnownTypes);

        TBuilder WithGenerateXmlObjects(bool generateXmlObjects);

        TBuilder WithIgnoreObsoleteProperties(bool ignoreObsoleteProperties);

        TBuilder WithAllowReferencesWithProperties(bool allowReferencesWithProperties);

        TBuilder WithGenerateEnumMappingDescription(bool generateEnumMappingDescription);

        TBuilder WithAlwaysAllowAdditionalObjectProperties(bool alwaysAllowAdditionalObjectProperties);

        TBuilder WithGenerateExamples(bool generateExamples);

        TBuilder WithSchemaType(SchemaType schemaType);

        TBuilder WithExcludedTypeNames(string[] excludedTypeNames);

        TBuilder WithUseXmlDocumentation(bool useXmlDocumentation);

        TBuilder WithResolveExternalXmlDocumentation(bool resolveExternalXmlDocumentation);

        TBuilder WithXmlDocumentationFormatting(XmlDocsFormattingMode xmlDocumentationFormatting);

        TBuilder WithTypeNameGenerator(ITypeNameGenerator typeNameGenerator);

        TBuilder WithSchemaNameGenerator(ISchemaNameGenerator schemaNameGenerator);

        TBuilder WithReflectionService(IReflectionService reflectionService);

        TBuilder WithTypeMappers(ICollection<ITypeMapper> typeMappers);

        TBuilder WithSchemaProcessors(ICollection<ISchemaProcessor> schemaProcessors);

        TBuilder WithGenerateCustomNullableProperties(bool generateCustomNullableProperties);
    }
}
