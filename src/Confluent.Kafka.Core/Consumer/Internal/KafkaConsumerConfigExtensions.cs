using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class KafkaConsumerConfigExtensions
    {
        public static bool HasTopicSubscriptions(this IKafkaConsumerConfig consumerConfig)
        {
            var hasTopicSubscriptions = consumerConfig is not null &&
                consumerConfig.TopicSubscriptions is not null &&
                consumerConfig.TopicSubscriptions.Any(topic => !string.IsNullOrWhiteSpace(topic));

            return hasTopicSubscriptions;
        }

        public static bool HasPartitionAssignments(this IKafkaConsumerConfig consumerConfig)
        {
            var hasPartitionAssignments = consumerConfig is not null &&
                consumerConfig.PartitionAssignments is not null &&
                consumerConfig.PartitionAssignments.Any(assignment => assignment is not null);

            return hasPartitionAssignments;
        }

        public static IEnumerable<string> GetCurrentTopics(this IKafkaConsumerConfig consumerConfig)
        {
            var currentTopics = (consumerConfig?.TopicSubscriptions ?? Enumerable.Empty<string>())
                .Concat(consumerConfig?.PartitionAssignments?.Select(partition => partition?.Topic) ?? Enumerable.Empty<string>())
                .Where(topic => !string.IsNullOrWhiteSpace(topic))
                .Distinct(StringComparer.Ordinal);

            return currentTopics;
        }

        public static void OnSubscriptionsOrAssignmentsChanged(
            this IKafkaConsumerConfig consumerConfig,
            IEnumerable<string> topicSubscriptions,
            IEnumerable<TopicPartition> partitionAssignments)
        {
            if (consumerConfig is not IKafkaConsumerConfigHandler consumerConfigHandler)
            {
                throw new InvalidCastException($"{nameof(consumerConfig)} should be of type '{nameof(IKafkaConsumerConfigHandler)}'.");
            }

            consumerConfigHandler.UpdateTopicSubscriptions(topicSubscriptions);
            consumerConfigHandler.UpdatePartitionAssignments(partitionAssignments);
        }
    }
}
