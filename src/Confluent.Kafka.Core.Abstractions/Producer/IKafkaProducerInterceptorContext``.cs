namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerInterceptorContext<TKey, TValue>
    {
        TKey Key { get; }

        TValue Value { get; }

        Headers Headers { get; }

        TopicPartition TopicPartition { get; }

        IKafkaProducerConfig ProducerConfig { get; }
    }
}
