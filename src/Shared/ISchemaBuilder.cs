using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaBuilder
    {
        ISchemaBuilder WithConfigureRegisteredSchema(
            Action<IRegisteredSchemaBuilder> configureRegisteredSchema);

        ISchemaBuilder WithConfigureUnregisteredSchema(
            Action<IUnregisteredSchemaBuilder> configureUnregisteredSchema);
    }
}
