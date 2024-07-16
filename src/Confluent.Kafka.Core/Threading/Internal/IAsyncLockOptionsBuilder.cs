using System;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal interface IAsyncLockOptionsBuilder
    {
        IAsyncLockOptionsBuilder WithMaxDegreeOfParallelism(int maxDegreeOfParallelism);

        IAsyncLockOptionsBuilder WithHandleLockByKey(bool handleLockByKey);

        IAsyncLockOptionsBuilder WithLockKeyHandler(Func<AsyncLockContext, object> lockKeyHandler);
    }
}
