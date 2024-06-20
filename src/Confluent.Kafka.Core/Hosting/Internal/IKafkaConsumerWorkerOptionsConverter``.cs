namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal interface IKafkaConsumerWorkerOptionsConverter<TKey, TValue>
    {
        IKafkaConsumerWorkerOptions<TKey, TValue> ToOptions();
    }
}
