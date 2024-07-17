using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal interface ISchemaRegistryAvroSerializerFactory
    {
        SchemaRegistryAvroSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer,
            object serializerKey);

        SchemaRegistryAvroSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryAvroSerializerBuilder> configureSerializer);
    }
}
