using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaBuilder
    {
        ISchemaBuilder WithRegisteredSchema(
            Action<IRegisteredSchemaBuilder> configureRegisteredSchema);

        ISchemaBuilder WithUnregisteredSchema(
            Action<IUnregisteredSchemaBuilder> configureUnregisteredSchema);
    }
}
