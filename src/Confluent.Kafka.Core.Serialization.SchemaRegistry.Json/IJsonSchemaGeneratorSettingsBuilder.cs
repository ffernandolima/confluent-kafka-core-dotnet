#if !NET8_0_OR_GREATER

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public interface IJsonSchemaGeneratorSettingsBuilder : 
        IJsonSchemaGeneratorSettingsBuilder<IJsonSchemaGeneratorSettingsBuilder>
    { }
}

#endif