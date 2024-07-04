using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Idempotency
{
    public interface IIdempotencyHandler<TKey, TValue> : IDisposable
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task<bool> TryHandleAsync(TValue messageValue, CancellationToken cancellationToken = default);
    }
}
