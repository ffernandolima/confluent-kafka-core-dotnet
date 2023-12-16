using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class NewtonsoftJsonSerializerKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithNewtonsoftJsonSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithNewtonsoftJsonKeySerializer(configureSettings);

            builder.WithNewtonsoftJsonValueSerializer(configureSettings);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithNewtonsoftJsonKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = NewtonsoftJsonSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                configureSettings);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithNewtonsoftJsonValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = NewtonsoftJsonSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                configureSettings);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
