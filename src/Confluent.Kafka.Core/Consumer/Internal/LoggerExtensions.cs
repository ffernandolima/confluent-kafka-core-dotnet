using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The message #{MessageId} has been consumed successfully from the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageConsumptionSuccess(this ILogger logger, object messageId, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The end of the topic '{Topic}' and partition [{Partition}] has been reached.")]
        public static partial void LogEndOfTopicPartitionReached(this ILogger logger, string topic, Partition partition);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while consuming a message from the topic '{Topic}', partition [{Partition}] and offset @{Offset}. Reason: {Error}.")]
        public static partial void LogMessageConsumptionFailure(this ILogger logger, Exception exception, string topic, Partition partition, Offset offset, Error error);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred during the retry attempt {RetryAttempt} while consuming a message from the topic(s) '{Topics}'.")]
        public static partial void LogMessageConsumptionRetryFailure(this ILogger logger, Exception exception, int retryAttempt, IEnumerable<string> topics);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred during the retry attempt {RetryAttempt} while consuming a message from the topic '{Topic}', partition [{Partition}] and offset @{Offset}. Reason: {Error}.")]
        public static partial void LogMessageConsumptionRetryFailure(this ILogger logger, Exception exception, int retryAttempt, string topic, Partition partition, Offset offset, Error error);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while executing the interceptor 'OnConsume' for the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageConsumptionInterceptionFailure(this ILogger logger, Exception exception, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while executing the interceptor 'OnConsume' for the message #{MessageId}, topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageConsumptionInterceptionFailure(this ILogger logger, Exception exception, object messageId, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while producing a dead letter message - built from the topic '{SourceTopic}', partition [{SourcePartition}] and offset @{SourceOffset} - to the '{DeadLetterTopic}' topic.")]
        public static partial void LogDeadLetterProductionFailure(this ILogger logger, Exception exception, string sourceTopic, Partition sourcePartition, Offset sourceOffset, string deadLetterTopic);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while committing a message from the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageCommitFailure(this ILogger logger, Exception exception, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while committing a message from the topic '{Topic}', partition [{Partition}] and offset @{Offset}. Reason: {Error}.")]
        public static partial void LogMessageCommitFailure(this ILogger logger, Exception exception, string topic, Partition partition, Offset offset, Error error);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while committing the message #{MessageId} from the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageCommitFailure(this ILogger logger, Exception exception, object messageId, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while committing the message #{MessageId} from the topic '{Topic}', partition [{Partition}] and offset @{Offset}. Reason: {Error}.")]
        public static partial void LogMessageCommitFailure(this ILogger logger, Exception exception, object messageId, string topic, Partition partition, Offset offset, Error error);
    }
}
