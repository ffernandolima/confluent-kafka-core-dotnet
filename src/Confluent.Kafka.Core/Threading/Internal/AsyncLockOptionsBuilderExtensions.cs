using System;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal static class AsyncLockOptionsBuilderExtensions
    {
        public static IAsyncLockOptionsBuilder WithHandleLockByKey(this IAsyncLockOptionsBuilder builder, Func<bool> keyFactory)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (keyFactory is null)
            {
                throw new ArgumentNullException(nameof(keyFactory));
            }

            builder.WithHandleLockByKey(keyFactory.Invoke());

            return builder;
        }
    }
}
