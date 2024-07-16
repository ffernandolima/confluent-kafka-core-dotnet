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
                throw new ArgumentNullException(nameof(builder));
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
                throw new ArgumentNullException(nameof(builder));
            }

            var keyDeserializer = SchemaRegistryProtobufSerializerFactory.Instance.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
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
                throw new ArgumentNullException(nameof(builder));
            }

            var valueDeserializer = SchemaRegistryProtobufSerializerFactory.Instance.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSerializer,
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
