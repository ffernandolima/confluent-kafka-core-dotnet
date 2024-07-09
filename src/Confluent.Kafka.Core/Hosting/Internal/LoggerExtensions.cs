using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "The worker '{ServiceName}' is starting.")]
        public static partial void LogWorkerStarting(this ILogger logger, string serviceName);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "The worker '{ServiceName}' is executing.")]
        public static partial void LogWorkerExecuting(this ILogger logger, string serviceName);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "The worker '{ServiceName}' is stopping.")]
        public static partial void LogWorkerStopping(this ILogger logger, string serviceName);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "No available messages. Waiting for them to come.")]
        public static partial void LogNoAvailableMessages(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "No available slots. Waiting for them to be released.")]
        public static partial void LogNoAvailableSlots(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Delaying until {Delay:yyyy-MM-dd'T'HH:mm:ss'Z'zzz}.")]
        public static partial void LogDelayingUntil(this ILogger logger, DateTime delay);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The current thread - representing the message #{MessageId} processing - had been blocked for {Elapsed:hh:mm:ss} until it has entered the SemaphoreSlim.")]
        public static partial void LogCurrentThreadBlocked(this ILogger logger, object messageId, TimeSpan elapsed);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The idempotency mechanism is enabled. The message #{MessageId} will be processed only once.")]
        public static partial void LogIdempotencyEnabled(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The idempotency mechanism is disabled. The message #{MessageId} may end up being processed more than once.")]
        public static partial void LogIdempotencyDisabled(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "The message #{MessageId} has been reproduced once it contains the specific retry headers. The idempotency mechanism will be bypassed.")]
        public static partial void LogIdempotencyBypassed(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "The message #{MessageId} has already been processed.")]
        public static partial void LogMessageAlreadyProcessed(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The message #{MessageId} has been reproduced once it contains the specific retry headers and " +
                      "it will not be processed because the group id header value does not correspond to the configured group id value.")]
        public static partial void LogMessageProcessingSkip(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The retry strategy is enabled. Any exception during the message #{MessageId} processing will be retried.")]
        public static partial void LogRetryStrategyEnabled(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The retry strategy is disabled. Any exception may abort the message #{MessageId} processing immediately.")]
        public static partial void LogRetryStrategyDisabled(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Dispatched a background work item to process the message #{MessageId} from the topic '{Topic}', partition [{Partition}] and offset @{Offset}. " +
                      "Message Value: {@MessageValue}")]
        public static partial void LogWorkItemDispatched(this ILogger logger, object messageId, string topic, Partition partition, Offset offset, object messageValue);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The message #{MessageId} has been processed successfully.")]
        public static partial void LogMessageProcessingSuccess(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "The message #{MessageId} will not be produced to the retry topic since the configured filters have ruled out the possibility of production.")]
        public static partial void LogMessageProcessingNotRetriable(this ILogger logger, object messageId);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while executing the worker '{ServiceName}'.")]
        public static partial void LogWorkerExecutionFailure(this ILogger logger, Exception exception, string serviceName);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while handling background work items completion.")]
        public static partial void LogCompletionHandlingFailure(this ILogger logger, Exception exception);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while consuming a message.")]
        public static partial void LogMessageConsumptionFailure(this ILogger logger, Exception exception);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while handling consumption exceptions.")]
        public static partial void LogExceptionHandlingFailure(this ILogger logger, Exception exception);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred on retry {RetryAttempt} for the message #{MessageId} processing.")]
        public static partial void LogMessageProcessingRetryFailure(this ILogger logger, Exception exception, object messageId, int retryAttempt);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while processing the message #{MessageId}.")]
        public static partial void LogMessageProcessingFailure(this ILogger logger, Exception exception, object messageId);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while producing a dead letter message - built from the message #{MessageId} - to the '{DeadLetterTopic}' topic.")]
        public static partial void LogDeadLetterProductionFailure(this ILogger logger, Exception exception, object messageId, string deadLetterTopic);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An exception has occurred while producing a retry message - built from the message #{MessageId} - to the '{RetryTopic}' topic.")]
        public static partial void LogRetryProductionFailure(this ILogger logger, Exception exception, object messageId, string retryTopic);
    }
}
