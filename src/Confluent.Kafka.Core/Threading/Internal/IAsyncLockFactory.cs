using System;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal interface IAsyncLockFactory
    {
        AsyncLock CreateAsyncLock(Action<IAsyncLockOptionsBuilder> configureOptions);
        AsyncLock CreateAsyncLock(AsyncLockOptions options);
    }
}
