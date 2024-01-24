using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Idempotency
{
    public interface IIdempotencyHandler<TKey, TValue>
    {
        Task<bool> TryHandleAsync(TValue messageValue);
    }
}
