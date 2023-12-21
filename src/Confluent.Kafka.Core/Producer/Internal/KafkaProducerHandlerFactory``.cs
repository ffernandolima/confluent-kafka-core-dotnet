using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerHandlerFactory<TKey, TValue> : IKafkaProducerHandlerFactory<TKey, TValue>
    {
        private static readonly Type DefaultProducerHandlerFactoryType = typeof(KafkaProducerHandlerFactory<TKey, TValue>);

        private readonly ILogger _logger;

        public KafkaProducerHandlerFactory(ILoggerFactory loggerFactory, KafkaProducerHandlerFactoryOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            _logger = loggerFactory.CreateLogger(options.EnableLogging, DefaultProducerHandlerFactoryType);
        }

        public Action<IProducer<TKey, TValue>, string> CreateStatisticsHandler() => (producer, statistics) =>
        {
            if (producer is null || string.IsNullOrWhiteSpace(statistics))
            {
                return;
            }

            _logger.LogInformation("[StatisticsHandler] -> ProducerName: {ProducerName} | Statistics: {Statistics}",
                producer.Name, statistics);
        };

        public Action<IProducer<TKey, TValue>, Error> CreateErrorHandler() => (producer, error) =>
        {
            if (producer is null || error is null)
            {
                return;
            }

            _logger.LogError("[ErrorHandler] -> ProducerName: {ProducerName} | Code: {Code} | Reason: {Reason}",
                producer.Name, error.Code, error.Reason);
        };

        public Action<IProducer<TKey, TValue>, LogMessage> CreateLogHandler() => (producer, logMessage) =>
        {
            if (producer is null || logMessage is null)
            {
                return;
            }

            var logLevel = (LogLevel)logMessage.LevelAs(LogLevelType.MicrosoftExtensionsLogging);

            _logger.Log(logLevel, "[LogHandler] -> ProducerName: {ProducerName} | ClientName: {ClientName} | SysLogLevel: {SysLogLevel} | Facility: {Facility} | Message: {Message}",
                producer.Name, logMessage.Name, logMessage.Level, logMessage.Facility, logMessage.Message);
        };

        public Func<TValue, object> CreateMessageIdHandler() => (messageValue) =>
        {
            var messageId = (messageValue as IMessageValue)?.GetId();

            return messageId;
        };
    }
}
