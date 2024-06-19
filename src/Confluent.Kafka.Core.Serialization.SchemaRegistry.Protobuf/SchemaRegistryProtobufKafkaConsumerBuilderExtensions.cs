using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal;
using Google.Protobuf;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class SchemaRegistryProtobufKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryProtobufDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class, IMessage<TKey>, new()
                where TValue : class, IMessage<TValue>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithSchemaRegistryProtobufKeyDeserializer(configureSerializer, serializerKey);
            builder.WithSchemaRegistryProtobufValueDeserializer(configureSerializer, serializerKey);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryProtobufKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class, IMessage<TKey>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = SchemaRegistryProtobufSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                configureSerializer,
                serializerKey);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryProtobufValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TValue : class, IMessage<TValue>, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = SchemaRegistryProtobufSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                configureSerializer,
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
