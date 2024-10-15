#if !NET8_0_OR_GREATER

using Microsoft.Extensions.Configuration;
using NJsonSchema.Generation;
using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class JsonSchemaGeneratorSettingsBuilder :
        JsonSchemaGeneratorSettingsBuilder<
            JsonSchemaGeneratorSettings,
            IJsonSchemaGeneratorSettingsBuilder,
            JsonSchemaGeneratorSettingsBuilder>,
        IJsonSchemaGeneratorSettingsBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override IJsonSchemaGeneratorSettingsBuilder BuilderInstance => this;

        public JsonSchemaGeneratorSettingsBuilder(IConfiguration configuration = null)
            : base(configuration)
        { }

        protected override JsonSchemaGeneratorSettings CreateSubject() => new()
        {
            FlattenInheritanceHierarchy = true
        };

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

#endif