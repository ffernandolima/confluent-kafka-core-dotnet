using System;
using Confluent.Kafka.Core.Internal;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    public static partial class KafkaConsumerBuilderExtensions
    {
        internal static IConsumer<TKey, TValue> BuildUnderlyingConsumer<TKey, TValue>(this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder)
        {
            if (consumerBuilder is not IConsumerBuilder<TKey, TValue> underlyingConsumerBuilder)
            {
                throw new InvalidCastException(
                    $"{nameof(consumerBuilder)} should be of type '{typeof(IConsumerBuilder<TKey, TValue>).ExtractTypeName()}'.");
            }

            var underlyingConsumer = underlyingConsumerBuilder.Build();

            return underlyingConsumer;
        }

        internal static IKafkaConsumerOptions<TKey, TValue> ToOptions<TKey, TValue>(this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder)
        {
            if (consumerBuilder is not IKafkaConsumerOptionsConverter<TKey, TValue> converter)
            {
                throw new InvalidCastException(
                    $"{nameof(consumerBuilder)} should be of type '{typeof(IKafkaConsumerOptionsConverter<TKey, TValue>).ExtractTypeName()}'.");
            }

            var options = converter.ToOptions();

            return options;
        }
    }
}
