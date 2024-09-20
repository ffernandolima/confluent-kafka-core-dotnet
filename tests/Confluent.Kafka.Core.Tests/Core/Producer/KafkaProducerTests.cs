using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Tests.Core.Diagnostics;
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
    public class KafkaProducerTests : IDisposable
    {
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "production-test-topic";

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly KafkaProducerConfig _producerConfig;
        private readonly IKafkaProducer<Null, string> _producer;

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

            _producerConfig = new KafkaProducerConfig
            {
                BootstrapServers = BootstrapServers,
                DefaultTopic = Topic,
                DefaultTimeout = TimeSpan.FromSeconds(1),
                PollAfterProducing = true
            };

            _producer = new KafkaProducerBuilder<Null, string>(_producerConfig)
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .Build();
        }

        public void Dispose()
        {
            _producer?.Dispose();

            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Produce_WithMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

            var message = new Message<Null, string> { Value = "value1" };

            // Act
            _producer.Produce(message, deliveryReport =>
            {
                Assert.False(deliveryReport.Error.IsError, "Delivery should be successful");
                Assert.Equal(message.Key, deliveryReport.Message.Key);
                Assert.Equal(message.Value, deliveryReport.Message.Value);
            });

            _producer.Flush(_producerConfig.DefaultTimeout);

            // Assert
            Assert.True(activities.Count > 0);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Produce_WithTopicAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

            var topic = "production-test-topic";
            var message = new Message<Null, string> { Value = "value2" };

            // Act
            _producer.Produce(topic, message, deliveryReport =>
            {
                // Assert
                Assert.False(deliveryReport.Error.IsError, "Delivery should be successful");
                Assert.Equal(message.Key, deliveryReport.Message.Key);
                Assert.Equal(message.Value, deliveryReport.Message.Value);
            });

            _producer.Flush(_producerConfig.DefaultTimeout);

            Assert.True(activities.Count > 0);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Produce_WithTopicAndPartitionAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

            var topic = "production-test-topic";
            var partition = new Partition(0);
            var message = new Message<Null, string> { Value = "value3" };

            // Act
            _producer.Produce(topic, partition, message, deliveryReport =>
            {
                // Assert
                Assert.False(deliveryReport.Error.IsError, "Delivery should be successful");
                Assert.Equal(message.Key, deliveryReport.Message.Key);
                Assert.Equal(message.Value, deliveryReport.Message.Value);
            });

            _producer.Flush(_producerConfig.DefaultTimeout);

            Assert.True(activities.Count > 0);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ProduceAsync_WithMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

            var message = new Message<Null, string> { Value = "value4" };

            // Act
            var deliveryResult = await _producer.ProduceAsync(message);

            // Assert
            Assert.Equal(message.Key, deliveryResult.Message.Key);
            Assert.Equal(message.Value, deliveryResult.Message.Value);
            Assert.True(activities.Count > 0);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ProduceAsync_WithTopicAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

            var topic = "production-test-topic";
            var message = new Message<Null, string> { Value = "value5" };

            // Act
            var deliveryResult = await _producer.ProduceAsync(topic, message);

            // Assert
            Assert.Equal(message.Key, deliveryResult.Message.Key);
            Assert.Equal(message.Value, deliveryResult.Message.Value);
            Assert.True(activities.Count > 0);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ProduceAsync_WithTopicAndPartitionAndMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

            var topic = "production-test-topic";
            var partition = new Partition(0);
            var message = new Message<Null, string> { Value = "value6" };

            // Act
            var deliveryResult = await _producer.ProduceAsync(topic, partition, message);

            // Assert
            Assert.Equal(message.Key, deliveryResult.Message.Key);
            Assert.Equal(message.Value, deliveryResult.Message.Value);
            Assert.True(activities.Count > 0);

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
