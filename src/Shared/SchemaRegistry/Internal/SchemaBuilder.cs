using Confluent.SchemaRegistry;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class SchemaBuilder : ISchemaBuilder
    {
        public RegisteredSchema RegisteredSchema { get; private set; }
        public Schema UnregisteredSchema { get; private set; }

        public ISchemaBuilder WithRegisteredSchemaConfiguration(
            Action<IRegisteredSchemaBuilder> configureRegisteredSchema)
        {
            RegisteredSchema = RegisteredSchemaBuilder.Build(configureRegisteredSchema);
            return this;
        }

        public ISchemaBuilder WithUnregisteredSchemaConfiguration(
            Action<IUnregisteredSchemaBuilder> configureUnregisteredSchema)
        {
            UnregisteredSchema = UnregisteredSchemaBuilder.Build(configureUnregisteredSchema);
            return this;
        }

        public static SchemaBuilder Configure(Action<ISchemaBuilder> configureSchema)
        {
            var builder = new SchemaBuilder();

            configureSchema?.Invoke(builder);

            return builder;
        }
    }
}
