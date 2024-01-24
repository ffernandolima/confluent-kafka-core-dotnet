namespace Confluent.Kafka.Core.Producer.Internal
{
    internal interface IKafkaProducerOptionsConverter<TKey, TValue>
    {
        IKafkaProducerOptions<TKey, TValue> ToOptions();
    }
}
