using Confluent.Kafka.Core.Consumer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Confluent.Kafka.Core.Tests.Core.Extensions
{
    public static class KafkaConsumerExtensions
    {
        // A ceiling, not a delay: a warm broker returns on the first attempt. Consumer startup can
        // take seconds when the broker is under load.
        private static readonly TimeSpan DefaultDeadline = TimeSpan.FromSeconds(30);

        public static ConsumeResult<TKey, TValue> Consume<TKey, TValue>(
            this IKafkaConsumer<TKey, TValue> consumer,
            TimeSpan timeout,
            int retryCount)
        {
            return ConsumeUntil(
                consumer.Consume,
                result => result is not null,
                timeout,
                retryCount);
        }

        public static IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch<TKey, TValue>(
            this IKafkaConsumer<TKey, TValue> consumer,
            TimeSpan timeout,
            int retryCount)
        {
            return ConsumeUntil(
                consumer.ConsumeBatch,
                results => results is not null && results.Any(),
                timeout,
                retryCount);
        }

        /// <summary>
        /// Polls until <paramref name="isSatisfied"/> or the deadline passes.
        /// <paramref name="retryCount"/> is the minimum number of attempts and
        /// <paramref name="timeout"/> the backoff between them.
        /// </summary>
        private static TResult ConsumeUntil<TResult>(
            Func<TResult> consume,
            Func<TResult, bool> isSatisfied,
            TimeSpan timeout,
            int retryCount)
        {
            var deadline = Stopwatch.StartNew();
            var attempts = 0;

            TResult result;

            while (true)
            {
                result = consume.Invoke();
                attempts++;

                if (isSatisfied.Invoke(result))
                {
                    break;
                }

                if (attempts > retryCount && deadline.Elapsed >= DefaultDeadline)
                {
                    break;
                }

                Thread.Sleep(timeout);
            }

            return result;
        }
    }
}
