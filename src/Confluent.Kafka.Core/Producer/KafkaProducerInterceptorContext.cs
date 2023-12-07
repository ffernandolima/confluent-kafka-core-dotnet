namespace Confluent.Kafka.Core.Producer
{
    public sealed class KafkaProducerInterceptorContext<TKey, TValue> : IKafkaProducerInterceptorContext<TKey, TValue>
    {
        public TKey Key { get; init; }
        public TValue Value { get; init; }
        public Headers Headers { get; init; }
        public TopicPartition TopicPartition { get; init; }
        public IKafkaProducerConfig ProducerConfig { get; init; }
    }
}
