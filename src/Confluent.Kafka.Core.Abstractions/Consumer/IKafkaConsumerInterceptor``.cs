namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerInterceptor<TKey, TValue>
    {
        IKafkaConsumerInterceptorContext<TKey, TValue> OnConsume(IKafkaConsumerInterceptorContext<TKey, TValue> context);
    }
}
