namespace Confluent.Kafka.Core.Consumer
{
    public sealed class KafkaConsumerInterceptorContext<TKey, TValue> : IKafkaConsumerInterceptorContext<TKey, TValue>
    {
        public ConsumeResult<TKey, TValue> ConsumeResult { get; init; }
        public IKafkaConsumerConfig ConsumerConfig { get; init; }
    }
}
