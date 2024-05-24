using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface IJsonSchemaGeneratorSettingsBuilder
    {
        IJsonSchemaGeneratorSettingsBuilder WithDefaultReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultReferenceTypeNullHandling);

        IJsonSchemaGeneratorSettingsBuilder WithDefaultDictionaryValueReferenceTypeNullHandling(
            ReferenceTypeNullHandling defaultDictionaryValueReferenceTypeNullHandling);

        IJsonSchemaGeneratorSettingsBuilder WithGenerateAbstractProperties(bool generateAbstractProperties);

        IJsonSchemaGeneratorSettingsBuilder WithFlattenInheritanceHierarchy(bool flattenInheritanceHierarchy);

        IJsonSchemaGeneratorSettingsBuilder WithGenerateAbstractSchemas(bool generateAbstractSchemas);

        IJsonSchemaGeneratorSettingsBuilder WithGenerateKnownTypes(bool generateKnownTypes);

        IJsonSchemaGeneratorSettingsBuilder WithGenerateXmlObjects(bool generateXmlObjects);

        IJsonSchemaGeneratorSettingsBuilder WithIgnoreObsoleteProperties(bool ignoreObsoleteProperties);

        IJsonSchemaGeneratorSettingsBuilder WithAllowReferencesWithProperties(bool allowReferencesWithProperties);

        IJsonSchemaGeneratorSettingsBuilder WithGenerateEnumMappingDescription(bool generateEnumMappingDescription);

        IJsonSchemaGeneratorSettingsBuilder WithAlwaysAllowAdditionalObjectProperties(bool alwaysAllowAdditionalObjectProperties);

        IJsonSchemaGeneratorSettingsBuilder WithGenerateExamples(bool generateExamples);

        IJsonSchemaGeneratorSettingsBuilder WithSchemaType(SchemaType schemaType);

        IJsonSchemaGeneratorSettingsBuilder WithSerializerSettings(JsonSerializerSettings serializerSettings);

        IJsonSchemaGeneratorSettingsBuilder WithSerializerOptions(object serializerOptions);

        IJsonSchemaGeneratorSettingsBuilder WithExcludedTypeNames(string[] excludedTypeNames);

        IJsonSchemaGeneratorSettingsBuilder WithUseXmlDocumentation(bool useXmlDocumentation);

        IJsonSchemaGeneratorSettingsBuilder WithResolveExternalXmlDocumentation(bool resolveExternalXmlDocumentation);

        IJsonSchemaGeneratorSettingsBuilder WithTypeNameGenerator(ITypeNameGenerator typeNameGenerator);

        IJsonSchemaGeneratorSettingsBuilder WithSchemaNameGenerator(ISchemaNameGenerator schemaNameGenerator);

        IJsonSchemaGeneratorSettingsBuilder WithReflectionService(IReflectionService reflectionService);

        IJsonSchemaGeneratorSettingsBuilder WithTypeMappers(ICollection<ITypeMapper> typeMappers);

        IJsonSchemaGeneratorSettingsBuilder WithSchemaProcessors(ICollection<ISchemaProcessor> schemaProcessors);

        IJsonSchemaGeneratorSettingsBuilder WithGenerateCustomNullableProperties(bool generateCustomNullableProperties);
    }
}
