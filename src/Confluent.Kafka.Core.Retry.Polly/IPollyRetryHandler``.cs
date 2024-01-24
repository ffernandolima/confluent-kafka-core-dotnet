namespace Confluent.Kafka.Core.Retry.Polly
{
    public interface IPollyRetryHandler<TKey, TValue> : IRetryHandler<TKey, TValue> // TODO: Review the need of this interface
    { }
}
