using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class NewtonsoftJsonSerializerKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithNewtonsoftJsonDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithNewtonsoftJsonKeyDeserializer(configureSettings);

            builder.WithNewtonsoftJsonValueDeserializer(configureSettings);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithNewtonsoftJsonKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = NewtonsoftJsonSerializerFactory.GetOrCreateSerializer<TKey>(builder.ServiceProvider, configureSettings);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithNewtonsoftJsonValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = NewtonsoftJsonSerializerFactory.GetOrCreateSerializer<TValue>(builder.ServiceProvider, configureSettings);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
