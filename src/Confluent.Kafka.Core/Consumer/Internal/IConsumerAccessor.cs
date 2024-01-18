namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal interface IConsumerAccessor<TKey, TValue>
    {
        IConsumer<TKey, TValue> UnderlyingConsumer { get; }
    }
}
