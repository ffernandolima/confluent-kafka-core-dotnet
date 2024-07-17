namespace Confluent.Kafka.Core.Producer.Internal
{
    internal interface IDeliveryReportFactory
    {
        DeliveryReport<TKey, TValue> CreateDefault<TKey, TValue>(
            TopicPartition partition,
            Message<TKey, TValue> message);
    }
}
