namespace Confluent.Kafka.Core.Producer.Internal
{
    internal interface IProducerAccessor<TKey, TValue>
    {
        IProducer<TKey, TValue> UnderlyingProducer { get; }
    }
}
