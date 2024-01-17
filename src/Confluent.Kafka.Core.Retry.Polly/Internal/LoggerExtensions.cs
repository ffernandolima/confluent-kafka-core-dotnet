using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The execution will not be retried as the configured filters have ruled out the possibility of retry.")]
        public static partial void LogExecutionNotRetriable(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred during the retry attempt {RetryAttempt}.")]
        public static partial void LogRetryExecutionFailure(this ILogger logger, Exception exception, int retryAttempt);
    }
}
