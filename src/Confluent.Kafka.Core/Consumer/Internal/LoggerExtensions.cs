using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The message #{MessageId} has been consumed successfully.")]
        public static partial void LogMessageConsumptionSuccess(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The end of the topic '{Topic}' and partition [{Partition}] has been reached.")]
        public static partial void LogEndOfTopicPartitionReached(this ILogger logger, string topic, Partition partition);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An error has occurred while consuming a message from the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageConsumptionFailure(this ILogger logger, Exception exception, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred during the retry attempt {RetryAttempt} while consuming a message.")]
        public static partial void LogMessageConsumptionRetryFailure(this ILogger logger, Exception exception, int retryAttempt);

        [LoggerMessage(
            Level = LogLevel.Error,
             Message = "An error has occurred while executing interceptor 'OnConsume' for the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageConsumptionInterceptionFailure(this ILogger logger, Exception exception, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An error has occurred while producing a dead letter message to the '{DeadLetterTopic}' topic.")]
        public static partial void LogDeadLetterProductionFailure(this ILogger logger, Exception exception, string deadLetterTopic);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An error has occurred while committing a message from the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageCommitmentFailure(this ILogger logger, Exception exception, string topic, Partition partition, Offset offset);
    }
}
