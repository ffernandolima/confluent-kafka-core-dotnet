using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Confluent.Kafka.Core.Serialization.ProtobufNet.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class ProtobufNetSerializerKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithProtobufNetDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithProtobufNetKeyDeserializer(configureOptions);

            builder.WithProtobufNetValueDeserializer(configureOptions);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithProtobufNetKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = ProtobufNetSerializerFactory.GetOrCreateSerializer<TKey>(builder.ServiceProvider, configureOptions);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithProtobufNetValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = ProtobufNetSerializerFactory.GetOrCreateSerializer<TValue>(builder.ServiceProvider, configureOptions);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}
