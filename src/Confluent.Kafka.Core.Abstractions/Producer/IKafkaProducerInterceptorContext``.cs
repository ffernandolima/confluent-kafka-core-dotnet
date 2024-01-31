namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerInterceptorContext<TKey, TValue>
    {
        TopicPartition TopicPartition { get; }

        Message<TKey, TValue> Message { get; }

        IKafkaProducerConfig ProducerConfig { get; }
    }
}
