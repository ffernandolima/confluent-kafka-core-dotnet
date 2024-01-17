using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Unable to handle idempotency as it has not been possible to find out the message id. Key: '{Key}', Message Type: '{MessageType}'.")]
        public static partial void LogMessageIdNotFound(this ILogger logger, string key, string messageType);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An error has occurred while handling idempotency with the Redis provider. Key: '{Key}'.")]
        public static partial void LogIdempotencyHandlingFailure(this ILogger logger, Exception exception, string key);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "{Affected} member(s) has/have expired the from Redis sorted set '{Key}'.")]
        public static partial void LogSortedSetMembersExpired(this ILogger logger, long affected, string key);
    }
}
