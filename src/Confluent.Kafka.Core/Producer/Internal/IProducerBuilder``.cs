namespace Confluent.Kafka.Core.Producer.Internal
{
    internal interface IProducerBuilder<TKey, TValue>
    {
        IProducer<TKey, TValue> Build();
    }
}
