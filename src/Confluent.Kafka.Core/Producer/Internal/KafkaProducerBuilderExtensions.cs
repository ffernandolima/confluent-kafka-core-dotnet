using System;
using Confluent.Kafka.Core.Internal;

namespace Confluent.Kafka.Core.Producer.Internal
{
    public static partial class KafkaProducerBuilderExtensions
    {
        internal static IProducer<TKey, TValue> BuildUnderlyingProducer<TKey, TValue>(this IKafkaProducerBuilder<TKey, TValue> producerBuilder)
        {
            if (producerBuilder is not IProducerBuilder<TKey, TValue> underlyingProducerBuilder)
            {
                throw new InvalidCastException(
                    $"{nameof(producerBuilder)} should be of type '{typeof(IProducerBuilder<TKey, TValue>).ExtractTypeName()}'.");
            }

            var underlyingProducer = underlyingProducerBuilder.Build();

            return underlyingProducer;
        }

        internal static IKafkaProducerOptions<TKey, TValue> ToOptions<TKey, TValue>(this IKafkaProducerBuilder<TKey, TValue> producerBuilder)
        {
            if (producerBuilder is not IKafkaProducerOptionsConverter<TKey, TValue> converter)
            {
                throw new InvalidCastException(
                    $"{nameof(producerBuilder)} should be of type '{typeof(IKafkaProducerOptionsConverter<TKey, TValue>).ExtractTypeName()}'.");
            }

            var options = converter.ToOptions();

            return options;
        }
    }
}
