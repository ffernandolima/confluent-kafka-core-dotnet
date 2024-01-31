namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerInterceptorContext<TKey, TValue> : IKafkaProducerInterceptorContext<TKey, TValue>
    {
        public TopicPartition TopicPartition { get; init; }
        public Message<TKey, TValue> Message { get; init; }
        public IKafkaProducerConfig ProducerConfig { get; init; }
    }
}
