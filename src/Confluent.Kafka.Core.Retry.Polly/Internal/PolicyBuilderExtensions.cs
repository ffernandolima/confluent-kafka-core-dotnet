using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal static class PolicyBuilderExtensions
    {
        public static RetryPolicy WaitAndRetry(
            this PolicyBuilder policyBuilder,
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            IEnumerable<TimeSpan> sleepDurations,
            Action<Exception, TimeSpan, int> onRetry)
        {
            if (policyBuilder is null)
            {
                throw new ArgumentNullException(nameof(policyBuilder), $"{nameof(policyBuilder)} cannot be null.");
            }

            if (onRetry is null)
            {
                throw new ArgumentNullException(nameof(onRetry), $"{nameof(onRetry)} cannot be null.");
            }

            return sleepDurations is null
                ? policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, TransformOnRetry(onRetry))
                : policyBuilder.WaitAndRetry(sleepDurations, TransformOnRetry(onRetry));
        }

        public static AsyncRetryPolicy WaitAndRetryAsync(
            this PolicyBuilder policyBuilder,
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            IEnumerable<TimeSpan> sleepDurations,
            Action<Exception, TimeSpan, int> onRetry)
        {
            if (policyBuilder is null)
            {
                throw new ArgumentNullException(nameof(policyBuilder), $"{nameof(policyBuilder)} cannot be null.");
            }

            if (onRetry is null)
            {
                throw new ArgumentNullException(nameof(onRetry), $"{nameof(onRetry)} cannot be null.");
            }

            return sleepDurations is null
                ? policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, TransformOnRetry(onRetry))
                : policyBuilder.WaitAndRetryAsync(sleepDurations, TransformOnRetry(onRetry));
        }

        private static Action<Exception, TimeSpan, int, Context> TransformOnRetry(Action<Exception, TimeSpan, int> onRetry)
            => (exception, timeSpan, retryAttempt, _) => onRetry.Invoke(exception, timeSpan, retryAttempt);
    }
}
