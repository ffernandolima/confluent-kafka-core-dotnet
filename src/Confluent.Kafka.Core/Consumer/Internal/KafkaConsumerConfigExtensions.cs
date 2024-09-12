using System.Linq;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class KafkaConsumerConfigExtensions
    {
        public static bool HasTopicSubscriptions(this IKafkaConsumerConfig consumerConfig)
        {
            var hasTopicSubscriptions = consumerConfig?.TopicSubscriptions is not null &&
                                        consumerConfig.TopicSubscriptions.Any(topic => !string.IsNullOrWhiteSpace(topic));

            return hasTopicSubscriptions;
        }

        public static bool HasPartitionAssignments(this IKafkaConsumerConfig consumerConfig)
        {
            var hasPartitionAssignments = consumerConfig?.PartitionAssignments is not null &&
                                          consumerConfig.PartitionAssignments.Any(assignment => assignment is not null);

            return hasPartitionAssignments;
        }
    }
}
