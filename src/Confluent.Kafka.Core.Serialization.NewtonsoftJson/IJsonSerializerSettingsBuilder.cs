using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Confluent.Kafka.Core.Serialization.NewtonsoftJson
{
    public interface IJsonSerializerSettingsBuilder
    {
        IJsonSerializerSettingsBuilder FromConfiguration(string sectionKey);

        IJsonSerializerSettingsBuilder WithReferenceLoopHandling(ReferenceLoopHandling referenceLoopHandling);

        IJsonSerializerSettingsBuilder WithMissingMemberHandling(MissingMemberHandling missingMemberHandling);

        IJsonSerializerSettingsBuilder WithObjectCreationHandling(ObjectCreationHandling objectCreationHandling);

        IJsonSerializerSettingsBuilder WithNullValueHandling(NullValueHandling nullValueHandling);

        IJsonSerializerSettingsBuilder WithDefaultValueHandling(DefaultValueHandling defaultValueHandling);

        IJsonSerializerSettingsBuilder WithConverters(IList<JsonConverter> converters);

        IJsonSerializerSettingsBuilder WithPreserveReferencesHandling(PreserveReferencesHandling preserveReferencesHandling);

        IJsonSerializerSettingsBuilder WithTypeNameHandling(TypeNameHandling typeNameHandling);

        IJsonSerializerSettingsBuilder WithMetadataPropertyHandling(MetadataPropertyHandling metadataPropertyHandling);

        IJsonSerializerSettingsBuilder WithTypeNameAssemblyFormatHandling(TypeNameAssemblyFormatHandling typeNameAssemblyFormatHandling);

        IJsonSerializerSettingsBuilder WithConstructorHandling(ConstructorHandling constructorHandling);

        IJsonSerializerSettingsBuilder WithContractResolver(IContractResolver contractResolver);

        IJsonSerializerSettingsBuilder WithEqualityComparer(IEqualityComparer equalityComparer);

        IJsonSerializerSettingsBuilder WithReferenceResolverProvider(Func<IReferenceResolver> referenceResolverProvider);

        IJsonSerializerSettingsBuilder WithTraceWriter(ITraceWriter traceWriter);

        IJsonSerializerSettingsBuilder WithSerializationBinder(ISerializationBinder serializationBinder);

        IJsonSerializerSettingsBuilder WithError(EventHandler<ErrorEventArgs> error);

        IJsonSerializerSettingsBuilder WithContext(StreamingContext context);

        IJsonSerializerSettingsBuilder WithDateFormatString(string dateFormatString);

        IJsonSerializerSettingsBuilder WithMaxDepth(int? maxDepth);

        IJsonSerializerSettingsBuilder WithFormatting(Formatting formatting);

        IJsonSerializerSettingsBuilder WithDateFormatHandling(DateFormatHandling dateFormatHandling);

        IJsonSerializerSettingsBuilder WithDateTimeZoneHandling(DateTimeZoneHandling dateTimeZoneHandling);

        IJsonSerializerSettingsBuilder WithDateParseHandling(DateParseHandling dateParseHandling);

        IJsonSerializerSettingsBuilder WithFloatFormatHandling(FloatFormatHandling floatFormatHandling);

        IJsonSerializerSettingsBuilder WithFloatParseHandling(FloatParseHandling floatParseHandling);

        IJsonSerializerSettingsBuilder WithStringEscapeHandling(StringEscapeHandling stringEscapeHandling);

        IJsonSerializerSettingsBuilder WithCulture(CultureInfo culture);

        IJsonSerializerSettingsBuilder WithCheckAdditionalContent(bool checkAdditionalContent);
    }
}
