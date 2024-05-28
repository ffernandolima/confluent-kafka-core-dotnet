using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaBuilder
    {
        ISchemaBuilder WithRegisteredSchemaConfiguration(
            Action<IRegisteredSchemaBuilder> configureRegisteredSchema);

        ISchemaBuilder WithUnregisteredSchemaConfiguration(
            Action<IUnregisteredSchemaBuilder> configureUnregisteredSchema);
    }
}
