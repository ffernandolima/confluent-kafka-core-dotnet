using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class SchemaRegistryJsonSerializerKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryJsonSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class
                where TValue : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.WithSchemaRegistryJsonKeySerializer(configureSerializer, serializerKey);
            builder.WithSchemaRegistryJsonValueSerializer(configureSerializer, serializerKey);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryJsonKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var keySerializer = SchemaRegistryJsonSerializerFactory.Instance.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSerializer,
                serializerKey);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryJsonValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TValue : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var valueSerializer = SchemaRegistryJsonSerializerFactory.Instance.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSerializer,
                serializerKey);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
