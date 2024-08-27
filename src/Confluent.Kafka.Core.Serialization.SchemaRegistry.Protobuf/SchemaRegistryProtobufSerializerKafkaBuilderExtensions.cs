using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class SchemaRegistryProtobufSerializerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddSchemaRegistryProtobufSerializer<T>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, ISchemaRegistryProtobufSerializerBuilder> configureSerializer,
            object serializerKey = null)
                where T : class, IMessage<T>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer));
            }

            builder.Services!.AddSchemaRegistryProtobufSerializer<T>(configureSerializer, serializerKey);

            return builder;
        }
    }
}
