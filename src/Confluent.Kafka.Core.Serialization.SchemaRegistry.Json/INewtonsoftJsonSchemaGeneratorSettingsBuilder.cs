#if NET8_0_OR_GREATER

using Newtonsoft.Json;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface INewtonsoftJsonSchemaGeneratorSettingsBuilder :
        IJsonSchemaGeneratorSettingsBuilder<INewtonsoftJsonSchemaGeneratorSettingsBuilder>
    {
        INewtonsoftJsonSchemaGeneratorSettingsBuilder WithSerializerSettings(JsonSerializerSettings serializerSettings);
    }
}

#endif