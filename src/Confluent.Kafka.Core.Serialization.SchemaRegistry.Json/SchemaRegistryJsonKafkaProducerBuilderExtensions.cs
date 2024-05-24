using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
{
    public static class SchemaRegistryJsonKafkaProducerBuilderExtensions
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
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
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
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = SchemaRegistryJsonSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                SchemaRegistryJsonSerializerBuilder.Configure(configureSerializer),
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
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = SchemaRegistryJsonSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                SchemaRegistryJsonSerializerBuilder.Configure(configureSerializer),
                serializerKey);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
