using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class SchemaRegistryAvroKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryAvroDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithSchemaRegistryAvroKeyDeserializer(configureSerializer, serializerKey);
            builder.WithSchemaRegistryAvroValueDeserializer(configureSerializer, serializerKey);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryAvroKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = SchemaRegistryAvroSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                configureSerializer,
                serializerKey);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryAvroValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = SchemaRegistryAvroSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                configureSerializer,
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
