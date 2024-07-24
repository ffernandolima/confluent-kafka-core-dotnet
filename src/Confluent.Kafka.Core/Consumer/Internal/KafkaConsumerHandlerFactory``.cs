using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerHandlerFactory<TKey, TValue> : IKafkaConsumerHandlerFactory<TKey, TValue>
    {
        private static readonly Type DefaultConsumerHandlerFactoryType = typeof(KafkaConsumerHandlerFactory<TKey, TValue>);

        private readonly ILogger _logger;

        public KafkaConsumerHandlerFactory(ILoggerFactory loggerFactory, KafkaConsumerHandlerFactoryOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _logger = loggerFactory.CreateLogger(options.EnableLogging, DefaultConsumerHandlerFactoryType);
        }

        public Action<IConsumer<TKey, TValue>, string> CreateStatisticsHandler() => (consumer, statistics) =>
        {
            if (consumer is null || string.IsNullOrWhiteSpace(statistics))
            {
                return;
            }

            _logger.LogInformation("[StatisticsHandler] -> ConsumerName: {ConsumerName} | Statistics: {Statistics}",
                consumer.Name, statistics);
        };

        public Action<IConsumer<TKey, TValue>, Error> CreateErrorHandler() => (consumer, error) =>
        {
            if (consumer is null || error is null)
            {
                return;
            }

            _logger.LogError("[ErrorHandler] -> ConsumerName: {ConsumerName} | Error: {Error}",
                consumer.Name, error);
        };

        public Action<IConsumer<TKey, TValue>, LogMessage> CreateLogHandler() => (consumer, logMessage) =>
        {
            if (consumer is null || logMessage is null)
            {
                return;
            }

            var logLevel = (LogLevel)logMessage.LevelAs(LogLevelType.MicrosoftExtensionsLogging);

            _logger.Log(logLevel, "[LogHandler] -> ConsumerName: {ConsumerName} | ClientName: {ClientName} | SysLogLevel: {SysLogLevel} | Facility: {Facility} | Message: {Message}",
                consumer.Name, logMessage.Name, logMessage.Level, logMessage.Facility, logMessage.Message);
        };

        public Action<IConsumer<TKey, TValue>, List<TopicPartition>> CreatePartitionsAssignedHandler() => (consumer, assignments) =>
        {
            if (consumer is null || assignments is null || assignments.Count == 0)
            {
                return;
            }

            _logger.LogInformation("[PartitionsAssignedHandler] -> ConsumerName: {ConsumerName} | Assignments: [ {Assignments} ]",
                consumer.Name, string.Join(",", assignments));
        };

        public Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> CreatePartitionsRevokedHandler() => (consumer, revokements) =>
        {
            if (consumer is null || revokements is null || revokements.Count == 0)
            {
                return;
            }

            _logger.LogInformation("[PartitionsRevokedHandler] -> ConsumerName: {ConsumerName} | Revokements: [ {Revokements} ]",
                consumer.Name, string.Join(",", revokements));
        };

        public Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> CreatePartitionsLostHandler() => (consumer, losses) =>
        {
            if (consumer is null || losses is null || losses.Count == 0)
            {
                return;
            }

            _logger.LogInformation("[PartitionsLostHandler] -> ConsumerName: {ConsumerName} | Losses: [ {Losses} ]",
                consumer.Name, string.Join(",", losses));
        };

        public Action<IConsumer<TKey, TValue>, CommittedOffsets> CreateOffsetsCommittedHandler() => (consumer, committedOffsets) =>
        {
            if (consumer is null || committedOffsets is null)
            {
                return;
            }

            if (committedOffsets.Error is not null)
            {
                var logLevel = committedOffsets.Error.IsError ? LogLevel.Error : LogLevel.Information;

                _logger.Log(logLevel, "[OffsetsCommittedHandler] -> ConsumerName: {ConsumerName} | OverallOperation: {OverallOperation}",
                    consumer.Name, committedOffsets.Error);
            }

            if (committedOffsets.Offsets is not null && committedOffsets.Offsets.Count > 0)
            {
                foreach (var offset in committedOffsets.Offsets.Where(offset => offset is not null))
                {
                    var logLevel = offset.Error is not null && offset.Error.IsError ? LogLevel.Error : LogLevel.Information;

                    _logger.Log(logLevel, "[OffsetsCommittedHandler] -> ConsumerName: {ConsumerName} | PerPartitionOperation: {PerPartitionOperation}",
                        consumer.Name, offset);
                }
            }
        };

        public Func<TValue, object> CreateMessageIdHandler() => (messageValue) =>
        {
            var messageId = (messageValue as IMessageValue)?.GetId();

            return messageId;
        };
    }
}
