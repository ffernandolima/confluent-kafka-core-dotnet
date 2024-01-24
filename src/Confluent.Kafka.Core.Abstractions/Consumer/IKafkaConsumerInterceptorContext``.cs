namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerInterceptorContext<TKey, TValue>
    {
        ConsumeResult<TKey, TValue> ConsumeResult { get; }

        IKafkaConsumerConfig ConsumerConfig { get; }
    }
}
