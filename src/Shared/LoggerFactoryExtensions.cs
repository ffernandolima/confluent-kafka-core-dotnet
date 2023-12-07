using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Confluent.Kafka.Core.Internal
{
    internal static class LoggerFactoryExtensions
    {
        public static ILogger CreateLogger(this ILoggerFactory loggerFactory, bool enableLogging, Type categoryType)
        {
            var logger = (enableLogging, loggerFactory) switch
            {
                (enableLogging: false, loggerFactory: _) or
                (enableLogging: true, loggerFactory: null) => NullLogger.Instance,
                (enableLogging: true, loggerFactory: not null) => loggerFactory.CreateLogger(categoryType)
            };

            return logger;
        }
    }
}
