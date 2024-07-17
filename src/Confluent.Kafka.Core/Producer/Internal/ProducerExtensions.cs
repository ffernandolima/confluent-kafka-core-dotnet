using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal static class ProducerExtensions
    {
        public static IKafkaProducer<TKey, TValue> ToKafkaProducer<TKey, TValue>(this IProducer<TKey, TValue> producer)
        {
            if (producer is not IKafkaProducer<TKey, TValue> kafkaProducer)
            {
                var kafkaProducerType = typeof(IKafkaProducer<TKey, TValue>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(producer)} should be of type '{kafkaProducerType}'.");
            }

            return kafkaProducer;
        }
    }
}
