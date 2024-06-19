using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class SchemaRegistryAvroKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryAvroSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithSchemaRegistryAvroKeySerializer(configureSerializer, serializerKey);
            builder.WithSchemaRegistryAvroValueSerializer(configureSerializer, serializerKey);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryAvroKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = SchemaRegistryAvroSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                configureSerializer,
                serializerKey);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithSchemaRegistryAvroValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = SchemaRegistryAvroSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                configureSerializer,
                serializerKey);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
