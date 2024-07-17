using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    public static partial class KafkaConsumerBuilderExtensions
    {
        internal static IConsumer<TKey, TValue> BuildUnderlyingConsumer<TKey, TValue>(this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder)
        {
            if (consumerBuilder is not IConsumerBuilder<TKey, TValue> underlyingConsumerBuilder)
            {
                var consumerBuilderType = typeof(IConsumerBuilder<TKey, TValue>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(consumerBuilder)} should be of type '{consumerBuilderType}'.");
            }

            var underlyingConsumer = underlyingConsumerBuilder.Build();

            return underlyingConsumer;
        }

        internal static IKafkaConsumerOptions<TKey, TValue> ToOptions<TKey, TValue>(this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder)
        {
            if (consumerBuilder is not IKafkaConsumerOptionsConverter<TKey, TValue> converter)
            {
                var optionsConverterType = typeof(IKafkaConsumerOptionsConverter<TKey, TValue>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(consumerBuilder)} should be of type '{optionsConverterType}'.");
            }

            var options = converter.ToOptions();

            return options;
        }
    }
}
