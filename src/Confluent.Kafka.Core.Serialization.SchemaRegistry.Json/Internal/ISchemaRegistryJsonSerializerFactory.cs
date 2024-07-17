using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal interface ISchemaRegistryJsonSerializerFactory
    {
        SchemaRegistryJsonSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer,
            object serializerKey)
                where T : class;

        SchemaRegistryJsonSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryJsonSerializerBuilder> configureSerializer)
                where T : class;
    }
}
