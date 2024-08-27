using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class SchemaRegistryAvroSerializerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddSchemaRegistryAvroSerializer<T>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, ISchemaRegistryAvroSerializerBuilder> configureSerializer,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer));
            }

            builder.Services!.AddSchemaRegistryAvroSerializer<T>(configureSerializer, serializerKey);

            return builder;
        }
    }
}
