using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace Confluent.Kafka.Core.Tests.Extensions
{
    public static class MockExtensions
    {
        public static void VerifyLog(this Mock<ILogger> mockLogger, LogLevel logLevel, Times times)
        {
            mockLogger.Verify(
                logger => logger.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((state, type) => true)),
                times);
        }
    }
}
