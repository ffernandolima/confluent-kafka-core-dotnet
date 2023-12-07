using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Debug, Message = "The execution will not be retried since the configured filters have ruled out the retry possibility.")]
        public static partial void LogExecutionNotRetriable(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "An exception has occurred on retry {RetryAttempt}.")]
        public static partial void LogRetryExecutionFailed(this ILogger logger, Exception exception, int retryAttempt);
    }
}
