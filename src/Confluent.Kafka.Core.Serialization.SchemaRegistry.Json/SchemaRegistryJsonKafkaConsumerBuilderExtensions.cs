using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class SchemaRegistryJsonKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryJsonDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class
                where TValue : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithSchemaRegistryJsonKeyDeserializer(configureSerializer, serializerKey);
            builder.WithSchemaRegistryJsonValueDeserializer(configureSerializer, serializerKey);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryJsonKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TKey : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = SchemaRegistryJsonSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                SchemaRegistryJsonSerializerBuilder.Configure(configureSerializer),
                serializerKey);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithSchemaRegistryJsonValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer = null,
            object serializerKey = null)
                where TValue : class
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = SchemaRegistryJsonSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                SchemaRegistryJsonSerializerBuilder.Configure(configureSerializer),
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}