using Confluent.Kafka.Core.Retry.Polly;
using Confluent.Kafka.Core.Retry.Polly.Internal;
using Confluent.Kafka.Core.Tests.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Retry
{
    public class PollyRetryHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        public PollyRetryHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();

            _mockLogger
                .Setup(logger => logger.IsEnabled(LogLevel.Error))
                .Returns(true);

            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);
        }

        [Fact]
        public void Handle_ShouldRetry_OnMatchingException()
        {
            // Arrange
            var retryCount = 3;
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                handler.Handle(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));

            // Assert
            Assert.Equal(retryCount + 1, retries);

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public async Task HandleAsync_ShouldRetry_OnMatchingException()
        {
            // Arrange
            var retryCount = 3;
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await handler.HandleAsync(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));

            // Assert
            Assert.Equal(retryCount + 1, retries);

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public void Handle_ShouldNotRetry_OnNonMatchingException()
        {
            // Arrange
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = 3,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;

            // Act
            Assert.Throws<ArgumentException>(() =>
                handler.Handle(_ =>
                {
                    retries++;
                    throw new ArgumentException();
                }));

            // Assert
            Assert.Equal(1, retries);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task HandleAsync_ShouldNotRetry_OnNonMatchingException()
        {
            // Arrange
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = 3,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;

            // Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await handler.HandleAsync(_ =>
                {
                    retries++;
                    throw new ArgumentException();
                }));

            // Assert
            Assert.Equal(1, retries);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Handle_ShouldRetry_Immediately()
        {
            // Arrange
            var retryCount = 3;
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                DelayProvider = _ => TimeSpan.Zero,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            Assert.Throws<InvalidOperationException>(() =>
                handler.Handle(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed < TimeSpan.FromMilliseconds(100));

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public async Task HandleAsync_ShouldRetry_Immediately()
        {
            // Arrange
            var retryCount = 3;
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                DelayProvider = _ => TimeSpan.Zero,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await handler.HandleAsync(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed < TimeSpan.FromMilliseconds(100));

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public void Handle_ShouldRetry_WithFixedDelay()
        {
            // Arrange
            var retryCount = 3;
            var delayPerAttempt = TimeSpan.FromMilliseconds(50);
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                DelayProvider = _ => delayPerAttempt,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            Assert.Throws<InvalidOperationException>(() =>
                handler.Handle(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed >= delayPerAttempt * retryCount);

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public async Task HandleAsync_ShouldRetry_WithFixedDelay()
        {
            // Arrange
            var retryCount = 3;
            var delayPerAttempt = TimeSpan.FromMilliseconds(50);
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                DelayProvider = _ => delayPerAttempt,
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await handler.HandleAsync(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed >= delayPerAttempt * retryCount);

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public void Handle_ShouldRetry_WithExponentialBackoff()
        {
            // Arrange
            var retryCount = 3;
            var initialDelay = TimeSpan.FromMilliseconds(10);
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                DelayProvider = retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * initialDelay.TotalMilliseconds),
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            Assert.Throws<InvalidOperationException>(() =>
                handler.Handle(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            var expectedExponentialDelay = TimeSpan.FromMilliseconds(10 + 20 + 40); // 70ms total delay

            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed >= expectedExponentialDelay);

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public async Task HandleAsync_ShouldRetry_WithExponentialBackoff()
        {
            // Arrange
            var retryCount = 3;
            var initialDelay = TimeSpan.FromMilliseconds(10);
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                DelayProvider = retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * initialDelay.TotalMilliseconds),
                ExceptionFilter = ex => ex is InvalidOperationException
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await handler.HandleAsync(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            var expectedExponentialDelay = TimeSpan.FromMilliseconds(10 + 20 + 40); // 70ms total delay

            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed >= expectedExponentialDelay);

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public void Handle_ShouldRetry_WithDelays()
        {
            // Arrange
            var retryCount = 3;
            var delays = new[] { TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(30) };
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                Delays = delays,
                ExceptionFilter = ex => ex is InvalidOperationException,
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            Assert.Throws<InvalidOperationException>(() =>
                handler.Handle(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed.TotalMilliseconds >= delays.Sum(delay => delay.TotalMilliseconds));

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }

        [Fact]
        public async Task HandleAsync_ShouldRetry_WithDelays()
        {
            // Arrange
            var retryCount = 3;
            var delays = new[] { TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(30) };
            var retryOptions = new PollyRetryHandlerOptions
            {
                RetryCount = retryCount,
                Delays = delays,
                ExceptionFilter = ex => ex is InvalidOperationException,
            };

            var handler = new PollyRetryHandler<Null, string>(_mockLoggerFactory.Object, retryOptions);
            var retries = 0;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await handler.HandleAsync(_ =>
                {
                    retries++;
                    throw new InvalidOperationException();
                }));
            stopwatch.Stop();

            // Assert
            Assert.Equal(retryCount + 1, retries);

            Assert.True(stopwatch.Elapsed.TotalMilliseconds >= delays.Sum(delay => delay.TotalMilliseconds));

            _mockLogger.VerifyLog(LogLevel.Error, Times.AtLeast(retryCount));
        }
    }
}
