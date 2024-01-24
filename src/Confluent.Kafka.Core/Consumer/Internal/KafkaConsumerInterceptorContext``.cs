namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerInterceptorContext<TKey, TValue> : IKafkaConsumerInterceptorContext<TKey, TValue>
    {
        public IKafkaConsumerConfig ConsumerConfig { get; init; }
        public ConsumeResult<TKey, TValue> ConsumeResult { get; init; }
    }
}
