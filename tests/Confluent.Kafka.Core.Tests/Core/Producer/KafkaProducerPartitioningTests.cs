using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Tests.Core.Diagnostics;
using Confluent.Kafka.Core.Tests.Core.Fixtures;
using Confluent.Kafka.Core.Tests.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Producer
{
    public sealed class KafkaProducerPartitioningTests : IAsyncLifetime
    {
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "production-partitioning-test-topic";

        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly IKafkaProducer<string, string> _producer;

        private readonly KafkaTopicFixture _kafkaTopicFixture;

        public KafkaProducerPartitioningTests()
        {
            _mockLogger = new Mock<ILogger>();

            _mockLogger
                .Setup(logger => logger.IsEnabled(LogLevel.Error))
                .Returns(true);

            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);

            _producer = new KafkaProducerBuilder<string, string>(
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
                [Topic],
                numPartitions: 3);
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
        public void Produce_SameKeyMessagesGoToSamePartition()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                var topicKvp = activity?.Tags.SingleOrDefault(tag => tag.Key == SemanticConventions.Messaging.DestinationName) ?? default;
                if (topicKvp.Value == Topic && activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var key = "key1";

            var value1 = "value1";
            var value2 = "value2";

            var partition1 = Partition.Any;
            var partition2 = Partition.Any;

            // Act
            _producer.Produce(
                new Message<string, string> { Key = key, Value = value1 },
                deliveryResult1 =>
                {
                    Assert.False(deliveryResult1.Error.IsError, "Delivery should be successful");
                    Assert.Equal(key, deliveryResult1.Message.Key);
                    Assert.Equal(value1, deliveryResult1.Message.Value);

                    partition1 = deliveryResult1.Partition;
                });

            _producer.Produce(
                new Message<string, string> { Key = key, Value = value2 },
                deliveryResult2 =>
                {
                    Assert.False(deliveryResult2.Error.IsError, "Delivery should be successful");
                    Assert.Equal(key, deliveryResult2.Message.Key);
                    Assert.Equal(value2, deliveryResult2.Message.Value);

                    partition2 = deliveryResult2.Partition;
                });

            _producer.Flush(DefaultTimeout);

            // Assert
            Assert.True(partition1 != Partition.Any);
            Assert.True(partition2 != Partition.Any);

            Assert.Equal(partition1, partition2);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ProduceAsync_SameKeyMessagesGoToSamePartition()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                var topicKvp = activity?.Tags.SingleOrDefault(tag => tag.Key == SemanticConventions.Messaging.DestinationName) ?? default;
                if (topicKvp.Value == Topic && activity.Kind == ActivityKind.Producer)
                {
                    activities.Add(activity);
                }
            });

            var key = "key2";

            var value1 = "value1";
            var value2 = "value2";

            // Act
            var deliveryResult1 = await _producer.ProduceAsync(
                new Message<string, string> { Key = key, Value = value1 });

            var partition1 = deliveryResult1.Partition;

            var deliveryResult2 = await _producer.ProduceAsync(
                new Message<string, string> { Key = key, Value = value2 });

            var partition2 = deliveryResult2.Partition;

            // Assert
            Assert.True(partition1 != Partition.Any);
            Assert.True(partition2 != Partition.Any);

            Assert.Equal(partition1, partition2);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }
    }
}
