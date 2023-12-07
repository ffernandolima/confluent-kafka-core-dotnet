using Confluent.Kafka.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer
{
    public static class KafkaConsumerExtensions
    {
        public static IEnumerable<KafkaTopicPartitionLag> Lag<TKey, TValue>(this IKafkaConsumer<TKey, TValue> consumer)
        {
            if (consumer is null)
            {
                throw new ArgumentNullException(nameof(consumer), $"{nameof(consumer)} cannot be null.");
            }

            var lags = consumer.Assignment?.Select(assignment =>
            {
                var watermarkOffsets = consumer.GetWatermarkOffsets(assignment);

                var highOffset = watermarkOffsets?.High ?? Offset.Unset;

                if (highOffset == Offset.Unset)
                {
                    highOffset = 0;
                }

                var currentOffset = consumer.Position(assignment);

                if (currentOffset == Offset.Unset)
                {
                    currentOffset = 0;
                }

                var lag = new KafkaTopicPartitionLag(
                    assignment.Topic,
                    assignment.Partition,
                    highOffset - currentOffset);

                return lag;

            }) ?? Enumerable.Empty<KafkaTopicPartitionLag>();

            return lags.ToList();
        }
    }
}
