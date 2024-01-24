namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducer<TKey, TValue> : IProducer<TKey, TValue>
    {
        IKafkaProducerOptions<TKey, TValue> Options { get; }
    }
}
