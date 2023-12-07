namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerInterceptor<TKey, TValue>
    {
        IKafkaProducerInterceptorContext<TKey, TValue> OnProduce(IKafkaProducerInterceptorContext<TKey, TValue> context);
    }
}
