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
                throw new ArgumentNullException(nameof(builder));
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
                throw new ArgumentNullException(nameof(builder));
            }

            var keyDeserializer = NewtonsoftJsonSerializerFactory.Instance.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
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
                throw new ArgumentNullException(nameof(builder));
            }

            var valueDeserializer = NewtonsoftJsonSerializerFactory.Instance.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureSettings,
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
