using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Hosting.Retry;
using Confluent.Kafka.Core.Hosting.Retry.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry.Internal;
using Confluent.Kafka.Core.Serialization.JsonCore.Internal;
using Confluent.Kafka.Core.Tests.Core.Diagnostics;
using Confluent.Kafka.Core.Tests.Core.Extensions;
using Confluent.Kafka.Core.Tests.Core.Fixtures;
using Confluent.Kafka.Core.Tests.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Hosting
{
    using System.Text;

    public sealed class KafkaRetryConsumerWorkerTests : IAsyncLifetime
    {
        private const string BootstrapServers = "localhost:9092";

        private static readonly int DefaultRetryCount = 5;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan DefaultDelay = TimeSpan.FromSeconds(3);

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly Encoding _encoding;
        private readonly IKafkaProducer<byte[], KafkaMetadataMessage> _producer;

        private readonly KafkaTopicFixture _kafkaTopicFixture;

        public KafkaRetryConsumerWorkerTests()
        {
            _mockLogger = new Mock<ILogger>();

            _mockLogger
                .Setup(logger => logger.IsEnabled(LogLevel.Error))
                .Returns(true);

            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);

            _encoding = EncodingFactory.Instance.CreateDefault();

            _producer = CreateProducer<byte[], KafkaMetadataMessage>(
                serializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            _kafkaTopicFixture = new KafkaTopicFixture(
                BootstrapServers,
                Enum.GetValues<KafkaTopic>()
                    .Select(value => value.GetDescription()));
        }

        #region IAsyncLifetime

        public async Task InitializeAsync()
        {
            await _kafkaTopicFixture.InitializeAsync();
        }

        public async Task DisposeAsync()
        {
            _producer?.Dispose();

            await _kafkaTopicFixture.DisposeAsync();
        }

        #endregion IAsyncLifetime

        #region Stubs

        public enum KafkaTopic
        {
            [Description("source-test-topic-1")]
            SourceTestTopic1,

            [Description($"source-test-topic-1{KafkaRetryConstants.RetryTopicSuffix}")]
            SourceTestTopic1Retry,

            [Description("source-test-topic-2")]
            SourceTestTopic2,

            [Description($"source-test-topic-2{KafkaRetryConstants.RetryTopicSuffix}")]
            SourceTestTopic2Retry,

            [Description("source-test-topic-3")]
            SourceTestTopic3,

            [Description($"source-test-topic-3{KafkaRetryConstants.RetryTopicSuffix}")]
            SourceTestTopic3Retry,

            [Description($"source-test-topic-3{KafkaProducerConstants.DeadLetterTopicSuffix}")]
            SourceTestTopic3DeadLetter,
        }

        #endregion Stubs

        [Fact]
        public async Task ExecuteAsync_ShouldConsumeAndProduceToSourceTopic()
        {
            // Arrange
            var sourceTopic = KafkaTopic.SourceTestTopic1.GetDescription();
            var retryTopic = KafkaTopic.SourceTestTopic1Retry.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(sourceTopic, retryTopic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var retryWorker = CreateRetryWorker([retryTopic]);
            using var sourceConsumer = CreateConsumer<byte[], byte[]>([sourceTopic]);

            await ProduceAsync(retryTopic, "test-value");

            var retryWorkerImpl = retryWorker.ToImplementation<KafkaRetryConsumerWorker>();

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await retryWorker.StartAsync(cts.Token);
            await retryWorker.ExecuteAsync(cts.Token);

            var sourceMessage = sourceConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.True(
                retryWorkerImpl.WorkItems.IsEmpty ||
                retryWorkerImpl.WorkItems.All(workItem => workItem.IsHandled || workItem.IsCompleted));

            Assert.NotNull(sourceMessage);
            Assert.Equal(sourceTopic, sourceMessage.Topic);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ExecuteAsync_OnMaxRetryAttemptsReached_ShouldNotProduceToSourceTopic()
        {
            // Arrange
            var sourceTopic = KafkaTopic.SourceTestTopic2.GetDescription();
            var retryTopic = KafkaTopic.SourceTestTopic2Retry.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(sourceTopic, retryTopic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var retryWorker = CreateRetryWorker([retryTopic]);
            using var sourceConsumer = CreateConsumer<byte[], byte[]>([sourceTopic]);

            await ProduceAsync(
                retryTopic,
                "test-value",
                [new Header(KafkaRetryConstants.RetryCountKey, _encoding.GetBytes("1"))]);

            var retryWorkerImpl = retryWorker.ToImplementation<KafkaRetryConsumerWorker>();

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await retryWorker.StartAsync(cts.Token);
            await retryWorker.ExecuteAsync(cts.Token);

            var sourceMessage = sourceConsumer.Consume();

            // Assert
            Assert.True(
                retryWorkerImpl.WorkItems.IsEmpty ||
                retryWorkerImpl.WorkItems.All(workItem => workItem.IsHandled || workItem.IsCompleted));

            Assert.Null(sourceMessage);
            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ExecuteAsync_OnMaxRetryAttemptsReached_ShouldProduceToDeadLetterTopic()
        {
            // Arrange
            var sourceTopic = KafkaTopic.SourceTestTopic3.GetDescription();
            var retryTopic = KafkaTopic.SourceTestTopic3Retry.GetDescription();
            var deadLetterTopic = KafkaTopic.SourceTestTopic3DeadLetter.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(sourceTopic, retryTopic, deadLetterTopic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var retryWorker = CreateRetryWorker([retryTopic], enableDeadLetterTopic: true);
            using var sourceConsumer = CreateConsumer<byte[], byte[]>([sourceTopic]);
            using var deadLetterConsumer = CreateConsumer<byte[], KafkaMetadataMessage>(
                [deadLetterTopic],
                deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            await ProduceAsync(
               retryTopic,
               "test-value",
               [new Header(KafkaRetryConstants.RetryCountKey, _encoding.GetBytes("1"))]);

            var retryWorkerImpl = retryWorker.ToImplementation<KafkaRetryConsumerWorker>();

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await retryWorker.StartAsync(cts.Token);
            await retryWorker.ExecuteAsync(cts.Token);

            var sourceMessage = sourceConsumer.Consume();
            var deadLetterMessage = deadLetterConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.True(
                retryWorkerImpl.WorkItems.IsEmpty ||
                retryWorkerImpl.WorkItems.All(workItem => workItem.IsHandled || workItem.IsCompleted));

            Assert.Null(sourceMessage);

            Assert.NotNull(deadLetterMessage);
            Assert.Equal(deadLetterTopic, deadLetterMessage.Topic);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        private IKafkaRetryConsumerWorker CreateRetryWorker(
            IEnumerable<string> topics = null,
            bool? enableDeadLetterTopic = null,
            TimeSpan? retryTopicDelay = null)
        {
            var builder = new KafkaRetryConsumerWorkerBuilder(
                new KafkaRetryConsumerWorkerConfig
                {
                    EnableDeadLetterTopic = enableDeadLetterTopic ?? false,
                    RetryTopicDelay = retryTopicDelay ?? DefaultTimeout
                })
                .WithConsumer(
                    CreateConsumer<byte[], KafkaMetadataMessage>(
                        topics,
                        deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>()))
                .WithSourceProducer(
                    CreateProducer<byte[], byte[]>())
                .WithLoggerFactory(_mockLoggerFactory.Object);

            if (enableDeadLetterTopic ?? false)
            {
                builder.WithDeadLetterProducer(
                    CreateProducer<byte[], KafkaMetadataMessage>(
                        serializer: CreateJsonCoreSerializer<KafkaMetadataMessage>()));
            }

            var retryWorker = builder.Build();

            return retryWorker;
        }

        private IKafkaProducer<TKey, TValue> CreateProducer<TKey, TValue>(
            string defaultTopic = null,
            TimeSpan? defaultTimeout = null,
            bool? pollAfterProducing = null,
            ISerializer<TValue> serializer = null)
        {
            var producer = new KafkaProducerBuilder<TKey, TValue>(
                new KafkaProducerConfig
                {
                    BootstrapServers = BootstrapServers,
                    DefaultTopic = defaultTopic,
                    DefaultTimeout = defaultTimeout ?? DefaultTimeout,
                    PollAfterProducing = pollAfterProducing ?? false
                })
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .WithValueSerializer(serializer)
                .Build();

            return producer;
        }

        private IKafkaConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(
            IEnumerable<string> topics = null,
            TimeSpan? defaultTimeout = null,
            IDeserializer<TValue> deserializer = null)
        {
            var consumer = new KafkaConsumerBuilder<TKey, TValue>(
                new KafkaConsumerConfig
                {
                    BootstrapServers = BootstrapServers,
                    GroupId = "test-retry-worker-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false,
                    EnableAutoOffsetStore = false,
                    PartitionAssignments = topics?.Select(topic => new TopicPartition(topic, new Partition(0))),
                    DefaultTimeout = defaultTimeout ?? DefaultTimeout,
                })
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .WithValueDeserializer(deserializer)
                .Build();

            return consumer;
        }

        private static JsonCoreSerializer<T> CreateJsonCoreSerializer<T>()
        {
            var serializer = new JsonCoreSerializer<T>(
                new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            return serializer;
        }

        private async Task ProduceAsync(string topic, string value, Headers headers = null)
        {
            await _producer.ProduceAsync(
                topic,
                new Message<byte[], KafkaMetadataMessage>
                {
                    Value = new KafkaMetadataMessage
                    {
                        SourceTopic = topic?.Replace(KafkaRetryConstants.RetryTopicSuffix, string.Empty),
                        SourceValue = _encoding.GetBytes(value)
                    },
                    Headers = headers
                });

            _producer.Flush(DefaultTimeout);
        }
    }
}
