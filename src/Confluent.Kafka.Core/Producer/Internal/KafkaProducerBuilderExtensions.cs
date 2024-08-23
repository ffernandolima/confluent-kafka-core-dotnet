using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    public static partial class KafkaProducerBuilderExtensions
    {
        internal static IProducer<TKey, TValue> BuildUnderlyingProducer<TKey, TValue>(this IKafkaProducerBuilder<TKey, TValue> producerBuilder)
        {
            if (producerBuilder is not IProducerBuilder<TKey, TValue> underlyingProducerBuilder)
            {
                var producerBuilderType = typeof(IProducerBuilder<TKey, TValue>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(producerBuilder)} should be of type '{producerBuilderType}'.");
            }

            var underlyingProducer = underlyingProducerBuilder.Build();

            return underlyingProducer;
        }
    }
}
