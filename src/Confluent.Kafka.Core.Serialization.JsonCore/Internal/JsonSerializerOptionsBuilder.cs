using Confluent.Kafka.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Confluent.Kafka.Core.Serialization.JsonCore.Internal
{
    internal sealed class JsonSerializerOptionsBuilder :
        FunctionalBuilder<JsonSerializerOptions, JsonSerializerOptionsBuilder>,
        IJsonSerializerOptionsBuilder
    {
        protected override JsonSerializerOptions CreateSubject() => new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public IJsonSerializerOptionsBuilder WithConverters(IList<JsonConverter> converters)
        {
            AppendAction(options =>
            {
                if (options.Converters is not null &&
                            converters is not null &&
                            converters.Any(converter => converter is not null))
                {
                    foreach (var converter in converters.Where(converter => converter is not null))
                    {
                        if (!options.Converters.Contains(converter))
                        {
                            options.Converters.Add(converter);
                        }
                    }
                }
            });
            return this;
        }

        public IJsonSerializerOptionsBuilder WithTypeInfoResolver(IJsonTypeInfoResolver typeInfoResolver)
        {
            AppendAction(options => options.TypeInfoResolver = typeInfoResolver);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithTypeInfoResolverChain(IList<IJsonTypeInfoResolver> typeInfoResolverChain)
        {
            AppendAction(options =>
            {
                if (options.TypeInfoResolverChain is not null &&
                            typeInfoResolverChain is not null &&
                            typeInfoResolverChain.Any(typeInfoResolver => typeInfoResolver is not null))
                {
                    foreach (var typeInfoResolver in typeInfoResolverChain.Where(typeInfoResolver => typeInfoResolver is not null))
                    {
                        if (!options.TypeInfoResolverChain.Contains(typeInfoResolver))
                        {
                            options.TypeInfoResolverChain.Add(typeInfoResolver);
                        }
                    }
                }
            });
            return this;
        }

        public IJsonSerializerOptionsBuilder WithAllowTrailingCommas(bool allowTrailingCommas)
        {
            AppendAction(options => options.AllowTrailingCommas = allowTrailingCommas);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithDefaultBufferSize(int defaultBufferSize)
        {
            AppendAction(options => options.DefaultBufferSize = defaultBufferSize);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithEncoder(JavaScriptEncoder encoder)
        {
            AppendAction(options => options.Encoder = encoder);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithDictionaryKeyPolicy(JsonNamingPolicy dictionaryKeyPolicy)
        {
            AppendAction(options => options.DictionaryKeyPolicy = dictionaryKeyPolicy);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithDefaultIgnoreCondition(JsonIgnoreCondition defaultIgnoreCondition)
        {
            AppendAction(options => options.DefaultIgnoreCondition = defaultIgnoreCondition);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithNumberHandling(JsonNumberHandling numberHandling)
        {
            AppendAction(options => options.NumberHandling = numberHandling);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithPreferredObjectCreationHandling(JsonObjectCreationHandling preferredObjectCreationHandling)
        {
            AppendAction(options => options.PreferredObjectCreationHandling = preferredObjectCreationHandling);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithIgnoreReadOnlyProperties(bool ignoreReadOnlyProperties)
        {
            AppendAction(options => options.IgnoreReadOnlyProperties = ignoreReadOnlyProperties);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithIgnoreReadOnlyFields(bool ignoreReadOnlyFields)
        {
            AppendAction(options => options.IgnoreReadOnlyFields = ignoreReadOnlyFields);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithIncludeFields(bool includeFields)
        {
            AppendAction(options => options.IncludeFields = includeFields);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithMaxDepth(int maxDepth)
        {
            AppendAction(options => options.MaxDepth = maxDepth);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithPropertyNamingPolicy(JsonNamingPolicy propertyNamingPolicy)
        {
            AppendAction(options => options.PropertyNamingPolicy = propertyNamingPolicy);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithPropertyNameCaseInsensitive(bool propertyNameCaseInsensitive)
        {
            AppendAction(options => options.PropertyNameCaseInsensitive = propertyNameCaseInsensitive);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithReadCommentHandling(JsonCommentHandling readCommentHandling)
        {
            AppendAction(options => options.ReadCommentHandling = readCommentHandling);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithUnknownTypeHandling(JsonUnknownTypeHandling unknownTypeHandling)
        {
            AppendAction(options => options.UnknownTypeHandling = unknownTypeHandling);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithUnmappedMemberHandling(JsonUnmappedMemberHandling unmappedMemberHandling)
        {
            AppendAction(options => options.UnmappedMemberHandling = unmappedMemberHandling);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithWriteIndented(bool writeIndented)
        {
            AppendAction(options => options.WriteIndented = writeIndented);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithReferenceHandler(ReferenceHandler referenceHandler)
        {
            AppendAction(options => options.ReferenceHandler = referenceHandler);
            return this;
        }

        internal static JsonSerializerOptions Build(Action<IJsonSerializerOptionsBuilder> configureOptions)
        {
            using var builder = new JsonSerializerOptionsBuilder();

            configureOptions?.Invoke(builder);

            var options = builder.Build();

            return options;
        }
    }
}
