namespace Confluent.Kafka.Core.Models
{
    public sealed class KafkaTopicPartitionLag
    {
        public string Topic { get; }
        public Partition Partition { get; }
        public long Lag { get; }

        public TopicPartition TopicPartition => new(Topic, Partition);

        public KafkaTopicPartitionLag(string topic, Partition partition, long lag)
        {
            Topic = topic;
            Partition = partition;
            Lag = lag;
        }
    }
}