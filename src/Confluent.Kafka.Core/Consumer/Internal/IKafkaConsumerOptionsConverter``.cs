namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal interface IKafkaConsumerOptionsConverter<TKey, TValue>
    {
        IKafkaConsumerOptions<TKey, TValue> ToOptions();
    }
}
