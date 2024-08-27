using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class SchemaRegistryJsonSerializerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddSchemaRegistryJsonSerializer<T>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, ISchemaRegistryJsonSerializerBuilder> configureSerializer,
            object serializerKey = null)
                where T : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer));
            }

            builder.Services!.AddSchemaRegistryJsonSerializer<T>(configureSerializer, serializerKey);

            return builder;
        }
    }
}
