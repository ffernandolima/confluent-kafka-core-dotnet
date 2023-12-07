namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public interface IRedisIdempotencyHandler<TKey, TValue> : IIdempotencyHandler<TKey, TValue> // TODO: Review the need of this interface
    { }
}
