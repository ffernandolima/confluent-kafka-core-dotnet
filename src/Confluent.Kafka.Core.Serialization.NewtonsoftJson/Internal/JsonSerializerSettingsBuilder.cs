using Confluent.Kafka.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal
{
    internal sealed class JsonSerializerSettingsBuilder :
        FunctionalBuilder<JsonSerializerSettings, JsonSerializerSettingsBuilder>,
        IJsonSerializerSettingsBuilder
    {
        protected override JsonSerializerSettings CreateSubject() => new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal } }
        };

        public IJsonSerializerSettingsBuilder WithReferenceLoopHandling(ReferenceLoopHandling referenceLoopHandling)
        {
            AppendAction(settings => settings.ReferenceLoopHandling = referenceLoopHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithMissingMemberHandling(MissingMemberHandling missingMemberHandling)
        {
            AppendAction(settings => settings.MissingMemberHandling = missingMemberHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithObjectCreationHandling(ObjectCreationHandling objectCreationHandling)
        {
            AppendAction(settings => settings.ObjectCreationHandling = objectCreationHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithNullValueHandling(NullValueHandling nullValueHandling)
        {
            AppendAction(settings => settings.NullValueHandling = nullValueHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithDefaultValueHandling(DefaultValueHandling defaultValueHandling)
        {
            AppendAction(settings => settings.DefaultValueHandling = defaultValueHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithConverters(IList<JsonConverter> converters)
        {
            AppendAction(settings =>
            {
                settings.Converters ??= new List<JsonConverter>();

                if (converters is not null && converters.Any(converter => converter is not null))
                {
                    foreach (var converter in converters.Where(converter => converter is not null))
                    {
                        if (!settings.Converters.Contains(converter))
                        {
                            settings.Converters.Add(converter);
                        }
                    }
                }
            });
            return this;
        }

        public IJsonSerializerSettingsBuilder WithPreserveReferencesHandling(PreserveReferencesHandling preserveReferencesHandling)
        {
            AppendAction(settings => settings.PreserveReferencesHandling = preserveReferencesHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithTypeNameHandling(TypeNameHandling typeNameHandling)
        {
            AppendAction(settings => settings.TypeNameHandling = typeNameHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithMetadataPropertyHandling(MetadataPropertyHandling metadataPropertyHandling)
        {
            AppendAction(settings => settings.MetadataPropertyHandling = metadataPropertyHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithTypeNameAssemblyFormatHandling(TypeNameAssemblyFormatHandling typeNameAssemblyFormatHandling)
        {
            AppendAction(settings => settings.TypeNameAssemblyFormatHandling = typeNameAssemblyFormatHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithConstructorHandling(ConstructorHandling constructorHandling)
        {
            AppendAction(settings => settings.ConstructorHandling = constructorHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithContractResolver(IContractResolver contractResolver)
        {
            AppendAction(settings => settings.ContractResolver = contractResolver);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithEqualityComparer(IEqualityComparer equalityComparer)
        {
            AppendAction(settings => settings.EqualityComparer = equalityComparer);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithReferenceResolverProvider(Func<IReferenceResolver> referenceResolverProvider)
        {
            AppendAction(settings => settings.ReferenceResolverProvider = referenceResolverProvider);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithTraceWriter(ITraceWriter traceWriter)
        {
            AppendAction(settings => settings.TraceWriter = traceWriter);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithSerializationBinder(ISerializationBinder serializationBinder)
        {
            AppendAction(settings => settings.SerializationBinder = serializationBinder);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithError(EventHandler<ErrorEventArgs> error)
        {
            AppendAction(settings => settings.Error = error);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithContext(StreamingContext context)
        {
            AppendAction(settings => settings.Context = context);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithDateFormatString(string dateFormatString)
        {
            AppendAction(settings => settings.DateFormatString = dateFormatString);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithMaxDepth(int? maxDepth)
        {
            AppendAction(settings => settings.MaxDepth = maxDepth);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithFormatting(Formatting formatting)
        {
            AppendAction(settings => settings.Formatting = formatting);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithDateFormatHandling(DateFormatHandling dateFormatHandling)
        {
            AppendAction(settings => settings.DateFormatHandling = dateFormatHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithDateTimeZoneHandling(DateTimeZoneHandling dateTimeZoneHandling)
        {
            AppendAction(settings => settings.DateTimeZoneHandling = dateTimeZoneHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithDateParseHandling(DateParseHandling dateParseHandling)
        {
            AppendAction(settings => settings.DateParseHandling = dateParseHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithFloatFormatHandling(FloatFormatHandling floatFormatHandling)
        {
            AppendAction(settings => settings.FloatFormatHandling = floatFormatHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithFloatParseHandling(FloatParseHandling floatParseHandling)
        {
            AppendAction(settings => settings.FloatParseHandling = floatParseHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithStringEscapeHandling(StringEscapeHandling stringEscapeHandling)
        {
            AppendAction(settings => settings.StringEscapeHandling = stringEscapeHandling);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithCulture(CultureInfo culture)
        {
            AppendAction(settings => settings.Culture = culture);
            return this;
        }

        public IJsonSerializerSettingsBuilder WithCheckAdditionalContent(bool checkAdditionalContent)
        {
            AppendAction(settings => settings.CheckAdditionalContent = checkAdditionalContent);
            return this;
        }

        internal static JsonSerializerSettings Build(Action<IJsonSerializerSettingsBuilder> configureSettings)
        {
            using var builder = new JsonSerializerSettingsBuilder();

            configureSettings?.Invoke(builder);

            var settings = builder.Build();

            return settings;
        }
    }
}
