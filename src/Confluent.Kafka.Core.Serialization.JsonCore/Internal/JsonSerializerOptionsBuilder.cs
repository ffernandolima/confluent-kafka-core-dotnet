using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
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
        public JsonSerializerOptionsBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        protected override JsonSerializerOptions CreateSubject() => new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public IJsonSerializerOptionsBuilder FromConfiguration(string sectionKey)
        {
            AppendAction(options =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    options = Bind(options, sectionKey);
                }
            });
            return this;
        }

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

        public IJsonSerializerOptionsBuilder WithAllowOutOfOrderMetadataProperties(bool allowOutOfOrderMetadataProperties)
        {
            AppendAction(options => options.AllowOutOfOrderMetadataProperties = allowOutOfOrderMetadataProperties);
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

        public IJsonSerializerOptionsBuilder WithIndentCharacter(char indentCharacter)
        {
            AppendAction(options => options.IndentCharacter = indentCharacter);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithIndentSize(int indentSize)
        {
            AppendAction(options => options.IndentSize = indentSize);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithReferenceHandler(ReferenceHandler referenceHandler)
        {
            AppendAction(options => options.ReferenceHandler = referenceHandler);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithNewLine(string newLine)
        {
            AppendAction(options => options.NewLine = newLine);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithRespectNullableAnnotations(bool respectNullableAnnotations)
        {
            AppendAction(options => options.RespectNullableAnnotations = respectNullableAnnotations);
            return this;
        }

        public IJsonSerializerOptionsBuilder WithRespectRequiredConstructorParameters(bool respectRequiredConstructorParameters)
        {
            AppendAction(options => options.RespectRequiredConstructorParameters = respectRequiredConstructorParameters);
            return this;
        }

        public static JsonSerializerOptions Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IJsonSerializerOptionsBuilder> configureOptions)
        {
            using var builder = new JsonSerializerOptionsBuilder(configuration);

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}
