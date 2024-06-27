using Confluent.Kafka.Core.Serialization.JsonCore;
using Confluent.Kafka.Core.Serialization.JsonCore.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class JsonCoreSerializerKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithJsonCoreDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithJsonCoreKeyDeserializer(configureOptions, serializerKey);
            builder.WithJsonCoreValueDeserializer(configureOptions, serializerKey);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithJsonCoreKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = JsonCoreSerializerFactory.Instance.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
                configureOptions,
                serializerKey);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithJsonCoreValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = JsonCoreSerializerFactory.Instance.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureOptions,
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
