using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class NewtonsoftJsonSerializerKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithNewtonsoftJsonSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithNewtonsoftJsonKeySerializer(configureSettings, serializerKey);
            builder.WithNewtonsoftJsonValueSerializer(configureSettings, serializerKey);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithNewtonsoftJsonKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = NewtonsoftJsonSerializerFactory.Instance.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSettings,
                serializerKey);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithNewtonsoftJsonValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = NewtonsoftJsonSerializerFactory.Instance.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSettings,
                serializerKey);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
