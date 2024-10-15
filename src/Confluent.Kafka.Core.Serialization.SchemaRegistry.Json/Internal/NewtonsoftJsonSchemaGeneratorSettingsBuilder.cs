#if NET8_0_OR_GREATER

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NJsonSchema.NewtonsoftJson.Generation;
using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class NewtonsoftJsonSchemaGeneratorSettingsBuilder :
        JsonSchemaGeneratorSettingsBuilder<
            NewtonsoftJsonSchemaGeneratorSettings,
            INewtonsoftJsonSchemaGeneratorSettingsBuilder,
            NewtonsoftJsonSchemaGeneratorSettingsBuilder>,
        INewtonsoftJsonSchemaGeneratorSettingsBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override INewtonsoftJsonSchemaGeneratorSettingsBuilder BuilderInstance => this;

        public NewtonsoftJsonSchemaGeneratorSettingsBuilder(IConfiguration configuration = null)
            : base(configuration)
        { }

        protected override NewtonsoftJsonSchemaGeneratorSettings CreateSubject() => new()
        {
            FlattenInheritanceHierarchy = true
        };

        public INewtonsoftJsonSchemaGeneratorSettingsBuilder WithSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            AppendAction(settings => settings.SerializerSettings = serializerSettings);
            return BuilderInstance;
        }

        public static NewtonsoftJsonSchemaGeneratorSettings Build(
            IConfiguration configuration,
            Action<INewtonsoftJsonSchemaGeneratorSettingsBuilder> configureSchemaGenerator)
        {
            using var builder = new NewtonsoftJsonSchemaGeneratorSettingsBuilder(configuration);

            configureSchemaGenerator?.Invoke(builder);

            var settings = builder.Build();

            return settings;
        }
    }
}

#endif