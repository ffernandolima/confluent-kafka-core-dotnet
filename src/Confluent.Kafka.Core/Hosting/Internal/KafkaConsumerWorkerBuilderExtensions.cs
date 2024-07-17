using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    public static partial class KafkaConsumerWorkerBuilderExtensions
    {
        internal static IKafkaConsumerWorkerOptions<TKey, TValue> ToOptions<TKey, TValue>(
            this IKafkaConsumerWorkerBuilder<TKey, TValue> workerBuilder)
        {
            if (workerBuilder is not IKafkaConsumerWorkerOptionsConverter<TKey, TValue> converter)
            {
                var optionsConverterType = typeof(IKafkaConsumerWorkerOptionsConverter<TKey, TValue>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(workerBuilder)} should be of type '{optionsConverterType}'.");
            }

            var options = converter.ToOptions();

            return options;
        }
    }
}
