using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Confluent.Kafka.Core.Serialization.JsonCore
{
    public interface IJsonSerializerOptionsBuilder
    {
        IJsonSerializerOptionsBuilder FromConfiguration(string sectionKey);

        IJsonSerializerOptionsBuilder WithConverters(IList<JsonConverter> converters);

        IJsonSerializerOptionsBuilder WithTypeInfoResolver(IJsonTypeInfoResolver typeInfoResolver);

        IJsonSerializerOptionsBuilder WithTypeInfoResolverChain(IList<IJsonTypeInfoResolver> typeInfoResolverChain);

        IJsonSerializerOptionsBuilder WithAllowTrailingCommas(bool allowTrailingCommas);

        IJsonSerializerOptionsBuilder WithDefaultBufferSize(int defaultBufferSize);

        IJsonSerializerOptionsBuilder WithEncoder(JavaScriptEncoder encoder);

        IJsonSerializerOptionsBuilder WithDictionaryKeyPolicy(JsonNamingPolicy dictionaryKeyPolicy);

        IJsonSerializerOptionsBuilder WithDefaultIgnoreCondition(JsonIgnoreCondition defaultIgnoreCondition);

        IJsonSerializerOptionsBuilder WithNumberHandling(JsonNumberHandling numberHandling);

        IJsonSerializerOptionsBuilder WithPreferredObjectCreationHandling(JsonObjectCreationHandling preferredObjectCreationHandling);

        IJsonSerializerOptionsBuilder WithIgnoreReadOnlyProperties(bool ignoreReadOnlyProperties);

        IJsonSerializerOptionsBuilder WithIgnoreReadOnlyFields(bool ignoreReadOnlyFields);

        IJsonSerializerOptionsBuilder WithIncludeFields(bool includeFields);

        IJsonSerializerOptionsBuilder WithMaxDepth(int maxDepth);

        IJsonSerializerOptionsBuilder WithPropertyNamingPolicy(JsonNamingPolicy propertyNamingPolicy);

        IJsonSerializerOptionsBuilder WithPropertyNameCaseInsensitive(bool propertyNameCaseInsensitive);

        IJsonSerializerOptionsBuilder WithReadCommentHandling(JsonCommentHandling readCommentHandling);

        IJsonSerializerOptionsBuilder WithUnknownTypeHandling(JsonUnknownTypeHandling unknownTypeHandling);

        IJsonSerializerOptionsBuilder WithUnmappedMemberHandling(JsonUnmappedMemberHandling unmappedMemberHandling);

        IJsonSerializerOptionsBuilder WithWriteIndented(bool writeIndented);

        IJsonSerializerOptionsBuilder WithReferenceHandler(ReferenceHandler referenceHandler);
    }
}
