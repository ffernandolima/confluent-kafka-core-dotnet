using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal interface IAsyncSemaphore : IDisposable
    {
        Task WaitAsync(CancellationToken cancellationToken);

        void Release();
    }
}
