﻿using Confluent.Kafka.Core.Serialization.JsonCore;
using Confluent.Kafka.Core.Serialization.JsonCore.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class JsonCoreSerializerKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithJsonCoreDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            builder.WithJsonCoreKeyDeserializer(configureOptions);

            builder.WithJsonCoreValueDeserializer(configureOptions);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithJsonCoreKeyDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var keyDeserializer = JsonCoreSerializerFactory.GetOrCreateSerializer<TKey>(builder.ServiceProvider, configureOptions);

            builder.WithKeyDeserializer(keyDeserializer);

            return builder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithJsonCoreValueDeserializer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var valueDeserializer = JsonCoreSerializerFactory.GetOrCreateSerializer<TValue>(builder.ServiceProvider, configureOptions);

            builder.WithValueDeserializer(valueDeserializer);

            return builder;
        }
    }
}