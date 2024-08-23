using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Hosting.Retry.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Delaying non-blocking retry until {Delay:yyyy-MM-dd'T'HH:mm:ss'Z'zzz}.")]
        public static partial void LogDelayingRetryUntil(this ILogger logger, DateTime delay);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The maximum number of {RetryAttempts} retry attempts has been reached, " +
                      "so that the message #{MessageId} will not be reproduced to the source topic '{SourceTopic}.")]
        public static partial void LogMaximumRetryAttemptsReached(this ILogger logger, int retryAttempts, object messageId, string sourceTopic);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The dead letter topic strategy is disabled for source topic '{SourceTopic}', " +
                      "so that the message #{MessageId} will not be produced.")]
        public static partial void LogDeadLetterTopicStrategyDisabled(this ILogger logger, string sourceTopic, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Producing the message #{MessageId} to the source topic '{SourceTopic}'. Attempt: {RetryAttempt}.")]
        public static partial void LogProducingSourceMessage(this ILogger logger, object messageId, string sourceTopic, int retryAttempt);

        [LoggerMessage(
           Level = LogLevel.Debug,
           Message = "Producing the message #{MessageId} to the dead letter topic '{DeadLetterTopic}' " +
                     "since it has reached the maximum number of {RetryAttempts} retry attempts.")]
        public static partial void LogProducingDeadLetterMessage(this ILogger logger, object messageId, string deadLetterTopic, int retryAttempts);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "A non-valid message has been received. No action will be performed.")]
        public static partial void LogNonValidMessageReceived(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "The header '{HeaderKey}', value ({HeaderValue}), from the message #{MessageId}, is not an integer as expected.")]
        public static partial void LogRetryHeaderUnexpectedType(this ILogger logger, string headerKey, string headerValue, object messageId);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "The header '{HeaderKey}', from the message #{MessageId}, could not be found.")]
        public static partial void LogRetryHeaderNotFound(this ILogger logger, string headerKey, object messageId);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "The configured source topic '{ConfiguredSourceTopic}' is different than " +
                      "the source topic '{ReceivedSourceTopic}' received from the retry message #{MessageId}.")]
        public static partial void LogUnmatchedSourceTopic(this ILogger logger, string configuredSourceTopic, string receivedSourceTopic, object messageId);
    }
}
