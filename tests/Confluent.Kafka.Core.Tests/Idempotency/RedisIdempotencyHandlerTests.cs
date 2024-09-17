using Confluent.Kafka.Core.Idempotency.Redis;
using Confluent.Kafka.Core.Idempotency.Redis.Internal;
using Confluent.Kafka.Core.Tests.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Idempotency
{
    public class RedisIdempotencyHandlerTests : IAsyncLifetime
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly IConnectionMultiplexer _multiplexer;
        private readonly RedisIdempotencyHandler<Null, IdempotencyMessage> _handler;
        private readonly RedisIdempotencyHandlerOptions<Null, IdempotencyMessage> _options;

        public RedisIdempotencyHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();

            _mockLogger
                .Setup(logger => logger.IsEnabled(LogLevel.Warning))
                .Returns(true);

            _mockLogger
                .Setup(logger => logger.IsEnabled(LogLevel.Error))
                .Returns(true);

            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);

            _multiplexer = ConnectionMultiplexer.Connect("localhost:6379");

            _options = new RedisIdempotencyHandlerOptions<Null, IdempotencyMessage>
            {
                GroupId = "test-group",
                ConsumerName = "test-consumer",
                MessageIdHandler = message => message.Id,
                ExpirationInterval = TimeSpan.FromSeconds(3),
                ExpirationDelay = TimeSpan.FromMilliseconds(100),
                EnableLogging = true
            };

            _handler = new RedisIdempotencyHandler<Null, IdempotencyMessage>(_mockLoggerFactory.Object, _multiplexer, _options);
        }

        #region IAsyncLifetime Members

        public async Task InitializeAsync()
        {
            await _handler.StartAsync();
        }

        public Task DisposeAsync()
        {
            _handler.Dispose();

            return Task.CompletedTask;
        }

        #endregion IAsyncLifetime Members

        #region Stubs

        public class IdempotencyMessage
        {
            public string Id { get; set; }
            public string Content { get; set; }
        }

        #endregion Stubs

        [Fact]
        public async Task TryHandleAsync_MessageIdIsNullOrEmpty_ShouldLogAndReturnTrue()
        {
            // Arrange
            var messageWithNullId = new IdempotencyMessage { Id = null, Content = "null-id-message" };
            var messageWithEmptyId = new IdempotencyMessage { Id = "", Content = "empty-id-message" };

            // Act
            var resultWithNullId = await _handler.TryHandleAsync(messageWithNullId);
            var resultWithEmptyId = await _handler.TryHandleAsync(messageWithEmptyId);

            // Assert
            Assert.True(resultWithNullId, "When MessageId is null, the handler should return true.");
            Assert.True(resultWithEmptyId, "When MessageId is empty, the handler should return true.");

            _mockLogger.VerifyLog(LogLevel.Warning, Times.Exactly(2));
        }

        [Fact]
        public async Task TryHandleAsync_NewMessage_ShouldSucceed()
        {
            // Arrange
            var message = new IdempotencyMessage { Id = "1", Content = "test-message" };

            // Act
            var result = await _handler.TryHandleAsync(message);

            // Assert
            Assert.True(result, "The message should be handled successfully.");
        }

        [Fact]
        public async Task TryHandleAsync_DuplicateMessage_ShouldReturnFalse()
        {
            // Arrange
            var message = new IdempotencyMessage { Id = "2", Content = "duplicate-message" };

            // Act
            var firstTry = await _handler.TryHandleAsync(message);
            var secondTry = await _handler.TryHandleAsync(message);

            // Assert
            Assert.True(firstTry, "The first message should be handled.");
            Assert.False(secondTry, "The second message should not be handled again.");
        }

        [Fact]
        public async Task Expiration_ShouldRemoveOldMessages()
        {
            // Arrange
            var message = new IdempotencyMessage { Id = "3", Content = "expiring-message" };

            // Act
            await _handler.TryHandleAsync(message);

            await Task.Delay(_options.ExpirationInterval + TimeSpan.FromSeconds(1));

            // Assert
            var database = _multiplexer.GetDatabase();

            var result = await database.SortedSetScoreAsync(
                $"Idempotency:GroupIds:{_options.GroupId}:Consumers:{_options.ConsumerName}",
                message.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task TryHandleAsync_HandlesCancellationGracefully()
        {
            // Arrange
            var message = new IdempotencyMessage { Id = "4", Content = "cancellation-test" };
            using var cts = new CancellationTokenSource();

            // Act
            cts.Cancel();
            var result = await _handler.TryHandleAsync(message, cts.Token);

            // Assert
            Assert.True(result, "Operation should return true even on cancellation.");

            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public async Task TryHandleAsync_WhenRedisUnavailable_ShouldHandleGracefully()
        {
            // Arrange
            var invalidMultiplexer = await ConnectionMultiplexer.ConnectAsync(
                "localhost:6380",
                options =>
                {
                    options.SyncTimeout = 1;
                    options.ConnectTimeout = 1;
                    options.AbortOnConnectFail = false;
                });

            var handlerWithInvalidRedis = new RedisIdempotencyHandler<Null, IdempotencyMessage>(
                _mockLoggerFactory.Object,
                invalidMultiplexer,
                _options);

            var message = new IdempotencyMessage { Id = "5", Content = "unavailable-test" };

            // Act
            var result = await handlerWithInvalidRedis.TryHandleAsync(message);

            // Assert
            Assert.True(result, "Handler should gracefully handle Redis being unavailable.");

            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }
    }
}
