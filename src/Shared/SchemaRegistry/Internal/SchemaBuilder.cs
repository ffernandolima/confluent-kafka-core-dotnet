using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class SchemaBuilder : ISchemaBuilder
    {
        private readonly IConfiguration _configuration;

        public RegisteredSchema RegisteredSchema { get; private set; }
        public Schema UnregisteredSchema { get; private set; }

        public SchemaBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ISchemaBuilder WithRegisteredSchema(
            Action<IRegisteredSchemaBuilder> configureRegisteredSchema)
        {
            RegisteredSchema = RegisteredSchemaBuilder.Build(_configuration, configureRegisteredSchema);
            return this;
        }

        public ISchemaBuilder WithUnregisteredSchema(
            Action<IUnregisteredSchemaBuilder> configureUnregisteredSchema)
        {
            UnregisteredSchema = UnregisteredSchemaBuilder.Build(_configuration, configureUnregisteredSchema);
            return this;
        }

        public static SchemaBuilder Configure(IConfiguration configuration, Action<ISchemaBuilder> configureSchema)
        {
            var builder = new SchemaBuilder(configuration);

            configureSchema?.Invoke(builder);

            return builder;
        }
    }
}
