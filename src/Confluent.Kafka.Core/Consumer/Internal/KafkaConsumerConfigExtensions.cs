using System.Linq;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class KafkaConsumerConfigExtensions
    {
        public static bool HasTopicSubscriptions(this IKafkaConsumerConfig consumerConfig)
            => consumerConfig is not null &&
               consumerConfig.TopicSubscriptions is not null &&
               consumerConfig.TopicSubscriptions.Any(topic => !string.IsNullOrWhiteSpace(topic));

        public static bool HasPartitionAssignments(this IKafkaConsumerConfig consumerConfig)
            => consumerConfig is not null &&
               consumerConfig.PartitionAssignments is not null &&
               consumerConfig.PartitionAssignments.Any(assignment => assignment is not null);
    }
}
