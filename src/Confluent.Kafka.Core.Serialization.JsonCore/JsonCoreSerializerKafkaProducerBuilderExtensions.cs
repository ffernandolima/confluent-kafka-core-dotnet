using Confluent.Kafka.Core.Serialization.JsonCore;
using Confluent.Kafka.Core.Serialization.JsonCore.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class JsonCoreSerializerKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithJsonCoreSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithJsonCoreKeySerializer(configureOptions);

            builder.WithJsonCoreValueSerializer(configureOptions);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithJsonCoreKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = JsonCoreSerializerFactory.GetOrCreateSerializer<TKey>(builder.ServiceProvider, configureOptions);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithJsonCoreValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = JsonCoreSerializerFactory.GetOrCreateSerializer<TValue>(builder.ServiceProvider, configureOptions);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
