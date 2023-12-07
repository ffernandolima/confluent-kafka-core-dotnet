using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "Cannot handle idempotency due to it has not been possible to find out the message identifier.")]
        public static partial void LogMessageIdentifierNotFound(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "An error has occurred while handling idempotency with Redis provider. Key: '{Key}'.")]
        public static partial void LogIdempotencyHandlingFailed(this ILogger logger, Exception exception, string key);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Expired {Affected} members from Redis sorted set '{Key}'.")]
        public static partial void LogSortedSetMembersExpired(this ILogger logger, long affected, string key);
    }
}
