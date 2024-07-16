using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class AsyncLockOptionsBuilder :
        FunctionalBuilder<AsyncLockOptions, AsyncLockOptionsBuilder>,
        IAsyncLockOptionsBuilder
    {
        public IAsyncLockOptionsBuilder WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
        {
            AppendAction(options => options.MaxDegreeOfParallelism = maxDegreeOfParallelism);
            return this;
        }

        public IAsyncLockOptionsBuilder WithHandleLockByKey(bool handleLockByKey)
        {
            AppendAction(options => options.HandleLockByKey = handleLockByKey);
            return this;
        }

        public IAsyncLockOptionsBuilder WithLockKeyHandler(Func<AsyncLockContext, object> lockKeyHandler)
        {
            AppendAction(options => options.LockKeyHandler = lockKeyHandler);
            return this;
        }

        public static AsyncLockOptions Build(Action<IAsyncLockOptionsBuilder> configureOptions)
        {
            using var builder = new AsyncLockOptionsBuilder();

            configureOptions?.Invoke(builder);

            var options = builder.Build();

            return options;
        }
    }
}
