using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal;
using Google.Protobuf;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class SchemaRegistryProtobufKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryProtobufSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class, IMessage<TKey>, new()
                where TValue : class, IMessage<TValue>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.WithSchemaRegistryProtobufKeySerializer(configureSerializer, serializerKey);
            builder.WithSchemaRegistryProtobufValueSerializer(configureSerializer, serializerKey);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryProtobufKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class, IMessage<TKey>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var keySerializer = SchemaRegistryProtobufSerializerFactory.Instance.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSerializer,
                serializerKey);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryProtobufValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TValue : class, IMessage<TValue>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var valueSerializer = SchemaRegistryProtobufSerializerFactory.Instance.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSerializer,
                serializerKey);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
