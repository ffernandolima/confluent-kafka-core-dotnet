using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Confluent.Kafka.Core.Serialization.ProtobufNet.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class ProtobufNetSerializerKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithProtobufNetDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithProtobufNetKeyDeserializer(configureOptions, serializerKey);
            builder.WithProtobufNetValueDeserializer(configureOptions, serializerKey);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithProtobufNetKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = ProtobufNetSerializerFactory.GetOrCreateSerializer<TKey>(
                builder.ServiceProvider,
                builder.Configuration,
                configureOptions,
                serializerKey);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithProtobufNetValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = ProtobufNetSerializerFactory.GetOrCreateSerializer<TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                configureOptions,
                serializerKey);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
