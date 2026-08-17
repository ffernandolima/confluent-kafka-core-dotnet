using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Tests.Core.Diagnostics;
using Confluent.Kafka.Core.Tests.Core.Extensions;
using Confluent.Kafka.Core.Tests.Core.Fixtures;
using Confluent.Kafka.Core.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
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
    public sealed class KafkaProducerTests : IClassFixture<KafkaProducerTests.TopicFixture>, IDisposable
    {
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "production-test-topic";

        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        // Flush returns the number of messages still in flight; delivery can take seconds under load.
        private static readonly TimeSpan FlushTimeout = TimeSpan.FromSeconds(30);

        public sealed class TopicFixture : KafkaTopicFixture
        {
            public TopicFixture()
                : base(BootstrapServers, [Topic])
            { }
        }

        /// <summary>
        /// Supplies the noop diagnostics manager, which never creates an activity. 
        /// The builder also resolves keyed services, so a plain mock is not enough.
        /// </summary>
        private sealed class NoopDiagnosticsServiceProvider : IServiceProvider, IKeyedServiceProvider
        {
            public object GetService(Type serviceType)
                => serviceType == typeof(IKafkaDiagnosticsManager) ? KafkaNoopDiagnosticsManager.Instance : null;

            public object GetKeyedService(Type serviceType, object serviceKey) => null;

            public object GetRequiredKeyedService(Type serviceType, object serviceKey)
                => throw new InvalidOperationException($"No service for type '{serviceType}' with key '{serviceKey}'.");
        }

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

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
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        [Fact]
        public void Produce_WithMessage_ProducesMessageSuccessfully()
        {
            // Arrange
            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(Topic) && activity.IsProducerKind())
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

            Assert.Equal(0, _producer.Flush(FlushTimeout));

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
                if (activity.HasTopic(Topic) && activity.IsProducerKind())
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

            Assert.Equal(0, _producer.Flush(FlushTimeout));

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
                if (activity.HasTopic(Topic) && activity.IsProducerKind())
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

            Assert.Equal(0, _producer.Flush(FlushTimeout));

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
                if (activity.HasTopic(Topic) && activity.IsProducerKind())
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
                if (activity.HasTopic(Topic) && activity.IsProducerKind())
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
                if (activity.HasTopic(Topic) && activity.IsProducerKind())
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

        /// <summary>
        /// The regression this design turns on. 
        /// The noop manager never creates an activity, which is also what happens when no listener is registered 
        /// and when sampling drops the span. 
        /// Metrics taken from the activity would silently undercount in exactly those cases, so they must not depend on it.
        /// </summary>
        [Fact]
        public async Task ProduceAsync_RecordsMetrics_WhenNoActivityIsCreated()
        {
            // Arrange
            using var producer = new KafkaProducerBuilder<Null, string>(
                new KafkaProducerConfig
                {
                    BootstrapServers = BootstrapServers,
                    DefaultTopic = Topic,
                    DefaultTimeout = DefaultTimeout,
                    PollAfterProducing = true
                })
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .WithServiceProvider(new NoopDiagnosticsServiceProvider())
                .Build();

            using var meterListener = new KafkaMeterListener();

            // Act
            await producer.ProduceAsync(new Message<Null, string> { Value = "metrics-value" });

            // Assert
            // The meter is process wide and test classes run in parallel, so the measurements are scoped to this topic.
            // Tests within a class run sequentially, and this topic belongs to this class.
            var duration = Assert.Single(ForThisTopic(meterListener, SemanticConventions.Metrics.ClientOperationDuration));
            var sent = Assert.Single(ForThisTopic(meterListener, SemanticConventions.Metrics.ClientSentMessages));

            Assert.Equal("s", duration.Unit);
            Assert.Equal("{message}", sent.Unit);
            Assert.True(duration.Value >= 0);
            Assert.Equal(1, sent.Value);

            foreach (var measurement in new[] { duration, sent })
            {
                Assert.Equal("kafka", measurement.GetTag(SemanticConventions.Messaging.System));
                Assert.Equal("publish", measurement.GetTag(SemanticConventions.Messaging.OperationName));
                Assert.Equal("send", measurement.GetTag(SemanticConventions.Messaging.OperationType));
                Assert.Equal(Topic, measurement.GetTag(SemanticConventions.Messaging.DestinationName));
                Assert.Equal("localhost", measurement.GetTag(SemanticConventions.Messaging.ServerAddress));
                Assert.Equal(9092, measurement.GetTag(SemanticConventions.Messaging.ServerPort));

                // Conditionally required: absent on success.
                Assert.False(measurement.HasTag(SemanticConventions.Messaging.ErrorType));
            }

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        /// <summary>
        /// The sync path records from the delivery report, not from the Produce call returning. 
        /// The partition proves it: the request carries Partition.Any, which is not reported, so a partition
        /// tag can only have come from the report. Recording earlier would also count a delivery that later failed as a success.
        /// </summary>
        [Fact]
        public void Produce_RecordsMetricsFromTheDeliveryReport()
        {
            // Arrange
            using var meterListener = new KafkaMeterListener();

            // Act
            _producer.Produce(new Message<Null, string> { Value = "sync-metrics-value" });

            Assert.Equal(0, _producer.Flush(FlushTimeout));

            // Assert
            var duration = Assert.Single(ForThisTopic(meterListener, SemanticConventions.Metrics.ClientOperationDuration));
            var sent = Assert.Single(ForThisTopic(meterListener, SemanticConventions.Metrics.ClientSentMessages));

            Assert.Equal(1, sent.Value);

            foreach (var measurement in new[] { duration, sent })
            {
                Assert.Equal("publish", measurement.GetTag(SemanticConventions.Messaging.OperationName));
                Assert.Equal("0", measurement.GetTag(SemanticConventions.Messaging.Kafka.DestinationPartitionId));
                Assert.False(measurement.HasTag(SemanticConventions.Messaging.ErrorType));
            }

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        private static IEnumerable<MeasurementRecord> ForThisTopic(KafkaMeterListener listener, string instrumentName)
            => listener.ByInstrument(instrumentName)
                       .Where(measurement => Equals(measurement.GetTag(SemanticConventions.Messaging.DestinationName), Topic));
    }
}
