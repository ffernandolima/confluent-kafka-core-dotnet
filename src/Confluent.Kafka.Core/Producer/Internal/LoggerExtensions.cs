using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Producing the message #{MessageId} to the topic '{Topic}' and partition [{Partition}].")]
        public static partial void LogProducingNewMessage(this ILogger logger, object messageId, string topic, Partition partition);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Callback events have been served for the message #{MessageId}.")]
        public static partial void LogCallbackEventsServed(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Callback events have not been served yet for the message #{MessageId}. The 'Status' has been set as 'PossiblyPersisted'.")]
        public static partial void LogCallbackEventsNotServed(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "'DeliveryReports' are disabled. The 'Status' for the message #{MessageId} has been set as 'PossiblyPersisted'.")]
        public static partial void LogDeliveryReportsDisabled(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The message #{MessageId} has been produced successfully to the topic '{Topic}', partition [{Partition}] and offset @{Offset}.")]
        public static partial void LogMessageProductionSuccess(this ILogger logger, object messageId, string topic, Partition partition, Offset offset);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while pooling for callback events.")]
        public static partial void LogPollFailure(this ILogger logger, Exception exception);

        [LoggerMessage(
           Level = LogLevel.Error,
           Message = "An exception has occurred while producing the message #{MessageId} to the topic '{Topic}' and partition [{Partition}]. Reason: {Error}.")]
        public static partial void LogMessageProductionFailure(this ILogger logger, object messageId, string topic, Partition partition, Error error);

        [LoggerMessage(
           Level = LogLevel.Error,
           Message = "An exception has occurred while producing the message #{MessageId} to the topic '{Topic}' and partition [{Partition}]. Reason: {Error}.")]
        public static partial void LogMessageProductionFailure(this ILogger logger, Exception exception, object messageId, string topic, Partition partition, Error error);

        [LoggerMessage(
           Level = LogLevel.Error,
           Message = "An exception has occurred during the retry attempt {RetryAttempt} while producing the message #{MessageId} to the topic '{Topic}' and partition [{Partition}].")]
        public static partial void LogMessageProductionRetryFailure(this ILogger logger, Exception exception, int retryAttempt, object messageId, string topic, Partition partition);

        [LoggerMessage(
           Level = LogLevel.Error,
           Message = "An exception has occurred during the retry attempt {RetryAttempt} while producing the message #{MessageId} to the topic '{Topic}' and partition [{Partition}]. Reason: {Error}.")]
        public static partial void LogMessageProductionRetryFailure(this ILogger logger, Exception exception, int retryAttempt, object messageId, string topic, Partition partition, Error error);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An error has occurred while executing the interceptor 'OnProduce' for the message #{MessageId}, topic '{Topic}' and partition [{Partition}].")]
        public static partial void LogMessageProductionInterceptionFailure(this ILogger logger, Exception exception, object messageId, string topic, Partition partition);
    }
}
