using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Confluent.Kafka.Core.Serialization.ProtobufNet.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class ProtobufNetSerializerKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithProtobufNetSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithProtobufNetKeySerializer(configureOptions, serializerKey);
            builder.WithProtobufNetValueSerializer(configureOptions, serializerKey);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithProtobufNetKeySerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keySerializer = ProtobufNetSerializerFactory.Instance.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
                configureOptions,
                serializerKey);

            builder.WithKeySerializer(keySerializer);

            return builder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithProtobufNetValueSerializer<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueSerializer = ProtobufNetSerializerFactory.Instance.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureOptions,
                serializerKey);

            builder.WithValueSerializer(valueSerializer);

            return builder;
        }
    }
}
