using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Threading.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal static class KafkaConsumerWorkerOptionsExtensions
    {
        private static readonly IEnumerable<Type> ExcludingTypes = [typeof(Null), typeof(Ignore)];

        internal static AsyncLockOptions ToLockOptions<TKey, TValue>(this IKafkaConsumerWorkerOptions<TKey, TValue> workerOptions)
        {
            if (workerOptions is null)
            {
                return null;
            }

            var asyncLockOptions = AsyncLockOptionsBuilder.Build(builder =>
            {
                builder.WithMaxDegreeOfParallelism(workerOptions.WorkerConfig!.MaxDegreeOfParallelism);

                builder.WithHandleLockByKey(() =>
                {
                    bool handleLockByKey;

                    if (workerOptions.WorkerConfig!.MaxDegreeOfParallelism <= 1)
                    {
                        handleLockByKey = false;
                    }
                    else if (workerOptions.WorkerConfig!.EnableMessageOrderGuarantee &&
                        workerOptions.MessageOrderGuaranteeKeyHandler is not null)
                    {
                        handleLockByKey = true;
                    }
                    else
                    {
                        handleLockByKey = !ExcludingTypes.Contains(typeof(TKey));
                    }

                    return handleLockByKey;
                });

                builder.WithLockKeyHandler(context =>
                {
                    if (context.Items!.TryGetValue(ConsumeResultConstants.ConsumeResult, out var contextItemValue) &&
                        contextItemValue is ConsumeResult<TKey, TValue> consumeResult)
                    {
                        return workerOptions.MessageOrderGuaranteeKeyHandler?.Invoke(consumeResult) ??
                               consumeResult.Message!.Key;
                    }

                    return null;
                });
            });

            return asyncLockOptions;
        }
    }
}
