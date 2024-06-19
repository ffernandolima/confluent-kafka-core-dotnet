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
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
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
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = SchemaRegistryProtobufSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
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
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = SchemaRegistryProtobufSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                configureSerializer,
                serializerKey);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
