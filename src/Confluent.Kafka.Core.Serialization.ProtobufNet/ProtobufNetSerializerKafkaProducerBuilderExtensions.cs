using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Confluent.Kafka.Core.Serialization.ProtobufNet.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class ProtobufNetSerializerKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithProtobufNetSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithProtobufNetKeySerializer(configureOptions);
            builder.WithProtobufNetValueSerializer(configureOptions);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithProtobufNetKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = ProtobufNetSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                configureOptions);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithProtobufNetValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = ProtobufNetSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                configureOptions);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
