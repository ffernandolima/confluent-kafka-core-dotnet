using Confluent.Kafka.Admin;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Tests.Core.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Producer
{
    public class KafkaProducerPartitioningTests : IAsyncLifetime
    {
        private const int Partitions = 3;
        private const string Servers = "localhost:9092";
        private const string Topic = "test-partitioning-topic";

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly KafkaProducerConfig _producerConfig;
        private readonly IKafkaProducer<string, string> _producer;

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

            _producerConfig = new KafkaProducerConfig
            {
                BootstrapServers = Servers,
                DefaultTopic = Topic,
                DefaultTimeout = TimeSpan.FromSeconds(1),
                PollAfterProducing = true
            };

            _producer = new KafkaProducerBuilder<string, string>(_producerConfig)
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .Build();
        }

        #region IAsyncLifetime Members

        public async Task InitializeAsync()
        {
            using var adminClient = new AdminClientBuilder(
                new AdminClientConfig { BootstrapServers = Servers }).Build();

            try
            {
                var topicSpecification = new TopicSpecification
                {
                    Name = Topic,
                    NumPartitions = Partitions,
                    ReplicationFactor = 1
                };

                await adminClient.CreateTopicsAsync([topicSpecification]);
            }
            catch (Exception ex) when (ex.Message.StartsWith("An error occurred creating topics"))
            { }
        }

        public Task DisposeAsync()
        {
            _producer?.Dispose();

            return Task.CompletedTask;
        }

        #endregion IAsyncLifetime Members

        [Fact]
        public void Produce_SameKeyMessagesGoToSamePartition()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

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

            _producer.Flush(_producerConfig.DefaultTimeout);

            // Assert
            Assert.True(partition1 != Partition.Any);
            Assert.True(partition2 != Partition.Any);

            Assert.Equal(partition1, partition2);

            Assert.True(activities.Count > 0);
        }

        [Fact]
        public async Task ProduceAsync_SameKeyMessagesGoToSamePartition()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activities.Add);

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

            Assert.True(activities.Count > 0);
        }
    }
}
