using System;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class AsyncLockFactory
    {
        private static readonly Lazy<AsyncLockFactory> Factory = new(
            () => new AsyncLockFactory(), isThreadSafe: true);

        public static AsyncLockFactory Instance => Factory.Value;

        private AsyncLockFactory()
        { }

        public AsyncLock CreateAsyncLock(Action<IAsyncLockOptionsBuilder> configureOptions)
        {
            var options = AsyncLockOptionsBuilder.Build(configureOptions);

            var asyncLock = CreateAsyncLock(options);

            return asyncLock;
        }

        public AsyncLock CreateAsyncLock(AsyncLockOptions options) => new(options);
    }
}
