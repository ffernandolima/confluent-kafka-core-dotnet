using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Tests.Core.Diagnostics;
using Confluent.Kafka.Core.Tests.Core.Fixtures;
using Confluent.Kafka.Core.Tests.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Producer
{
    public sealed class KafkaProducerTests : IAsyncLifetime
    {
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "production-test-topic";

        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly IKafkaProducer<Null, string> _producer;

        private readonly KafkaTopicFixture _kafkaTopicFixture;

        public KafkaProducerTests()
        {
            _mockLogger = new Mock<ILogger>();

            _mockLogger
                .Setup(logger => logger.IsEnabled(LogLevel.Error))
                .Returns(true);

            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);

            _producer = new KafkaProducerBuilder<Null, string>(
                new KafkaProducerConfig
                {
                    BootstrapServers = BootstrapServers,
                    DefaultTopic = Topic,
                    DefaultTimeout = DefaultTimeout,
                    PollAfterProducing = true
                })
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .Build();

            _kafkaTopicFixture = new KafkaTopicFixture(
                BootstrapServers,
                [Topic]);
        }

        #region IAsyncLifetime Members

        public async Task InitializeAsync()
        {
            await _kafkaTopicFixture.InitializeAsync();
        }

        public async Task DisposeAsync()
        {
            _producer?.Dispose();

            await _kafkaTopicFixture.DisposeAsync();
        }

        #endregion IAsyncLifetime Members

        [Fact]
        public void Produce_WithMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var message = new Message<Null, string> { Value = "value1" };

            // Act
            _producer.Produce(message, deliveryReport =>
            {
                Assert.False(deliveryReport.Error.IsError, "Delivery should be successful");
                Assert.Equal(message.Key, deliveryReport.Message.Key);
                Assert.Equal(message.Value, deliveryReport.Message.Value);
            });

            _producer.Flush(DefaultTimeout);

            // Assert
            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Produce_WithTopicAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var message = new Message<Null, string> { Value = "value2" };

            // Act
            _producer.Produce(Topic, message, deliveryReport =>
            {
                // Assert
                Assert.False(deliveryReport.Error.IsError, "Delivery should be successful");
                Assert.Equal(message.Key, deliveryReport.Message.Key);
                Assert.Equal(message.Value, deliveryReport.Message.Value);
            });

            _producer.Flush(DefaultTimeout);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Produce_WithTopicAndPartitionAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var partition = new Partition(0);
            var message = new Message<Null, string> { Value = "value3" };

            // Act
            _producer.Produce(Topic, partition, message, deliveryReport =>
            {
                // Assert
                Assert.False(deliveryReport.Error.IsError, "Delivery should be successful");
                Assert.Equal(message.Key, deliveryReport.Message.Key);
                Assert.Equal(message.Value, deliveryReport.Message.Value);
            });

            _producer.Flush(DefaultTimeout);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ProduceAsync_WithMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var message = new Message<Null, string> { Value = "value4" };

            // Act
            var deliveryResult = await _producer.ProduceAsync(message);

            // Assert
            Assert.Equal(message.Key, deliveryResult.Message.Key);
            Assert.Equal(message.Value, deliveryResult.Message.Value);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ProduceAsync_WithTopicAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var message = new Message<Null, string> { Value = "value5" };

            // Act
            var deliveryResult = await _producer.ProduceAsync(Topic, message);

            // Assert
            Assert.Equal(message.Key, deliveryResult.Message.Key);
            Assert.Equal(message.Value, deliveryResult.Message.Value);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ProduceAsync_WithTopicAndPartitionAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var partition = new Partition(0);
            var message = new Message<Null, string> { Value = "value6" };

            // Act
            var deliveryResult = await _producer.ProduceAsync(Topic, partition, message);

            // Assert
            Assert.Equal(message.Key, deliveryResult.Message.Key);
            Assert.Equal(message.Value, deliveryResult.Message.Value);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Produce_ThrowsExceptionOnNullMessage()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _producer.Produce(null));
        }

        [Fact]
        public async Task ProduceAsync_ThrowsExceptionOnNullMessage()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _producer.ProduceAsync(null));
        }
    }
}
