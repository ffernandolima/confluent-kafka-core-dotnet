using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class NewtonsoftJsonSerializerKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithNewtonsoftJsonDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithNewtonsoftJsonKeyDeserializer(configureSettings, serializerKey);
            builder.WithNewtonsoftJsonValueDeserializer(configureSettings, serializerKey);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithNewtonsoftJsonKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = NewtonsoftJsonSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                configureSettings,
                serializerKey);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithNewtonsoftJsonValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = NewtonsoftJsonSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                configureSettings,
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
