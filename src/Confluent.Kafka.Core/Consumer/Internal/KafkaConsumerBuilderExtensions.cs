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
    }
}
