namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal interface IConsumerBuilder<TKey, TValue>
    {
        IConsumer<TKey, TValue> Build();
    }
}
