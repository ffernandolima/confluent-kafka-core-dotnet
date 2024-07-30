using Confluent.Kafka.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class ConsumerExtensions
    {
        public static IKafkaConsumer<TKey, TValue> ToKafkaConsumer<TKey, TValue>(this IConsumer<TKey, TValue> consumer)
        {
            if (consumer is not IKafkaConsumer<TKey, TValue> kafkaConsumer)
            {
                var kafkaConsumerType = typeof(IKafkaConsumer<TKey, TValue>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(consumer)} should be of type '{kafkaConsumerType}'.");
            }

            return kafkaConsumer;
        }

        public static IEnumerable<string> GetCurrentTopics<TKey, TValue>(this IConsumer<TKey, TValue> consumer)
        {
            var currentTopics = (consumer?.Subscription ?? [])
                .Concat(consumer?.Assignment?.Select(partition => partition?.Topic) ?? [])
                .Where(topic => !string.IsNullOrWhiteSpace(topic))
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            return currentTopics;
        }
    }
}
