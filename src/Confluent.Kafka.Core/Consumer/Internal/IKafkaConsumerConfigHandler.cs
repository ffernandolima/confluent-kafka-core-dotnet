using System.Collections.Generic;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal interface IKafkaConsumerConfigHandler
    {
        void UpdateTopicSubscriptions(IEnumerable<string> topicSubscriptions);

        void UpdatePartitionAssignments(IEnumerable<TopicPartition> partitionAssignments);
    }
}
