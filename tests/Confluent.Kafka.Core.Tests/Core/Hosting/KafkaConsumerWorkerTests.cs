using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Hosting.Internal;
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

    public sealed class KafkaConsumerWorkerTests : IAsyncLifetime
    {
        private const string BootstrapServers = "localhost:9092";

        private static readonly int DefaultRetryCount = 5;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan DefaultDelay = TimeSpan.FromSeconds(3);

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly Encoding _encoding;
        private readonly IKafkaProducer<Null, byte[]> _producer;

        private readonly KafkaTopicFixture _kafkaTopicFixture;

        public KafkaConsumerWorkerTests()
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

            _producer = CreateProducer<Null, byte[]>();

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
            [Description("processing-test-topic")]
            ProcessingTestTopic,

            [Description("faulty-processing-test-topic-1")]
            FaultyProcessingTestTopic1,

            [Description($"faulty-processing-test-topic-1{KafkaRetryConstants.RetryTopicSuffix}")]
            FaultyProcessingTestTopic1Retry,

            [Description("faulty-processing-test-topic-2")]
            FaultyProcessingTestTopic2,

            [Description($"faulty-processing-test-topic-2{KafkaProducerConstants.DeadLetterTopicSuffix}")]
            FaultyProcessingTestTopic2DeadLetter,
        }

        public sealed class ConsumeResultHandler : IConsumeResultHandler<Null, string>
        {
            public static IConsumeResultHandler<Null, string> Create() => new ConsumeResultHandler();

            public Task HandleAsync(ConsumeResult<Null, string> consumeResult, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        public sealed class FaultyConsumeResultHandler : IConsumeResultHandler<Null, string>
        {
            public static IConsumeResultHandler<Null, string> Create() => new FaultyConsumeResultHandler();

            public Task HandleAsync(ConsumeResult<Null, string> consumeResult, CancellationToken cancellationToken)
            {
                throw new Exception("Faulty Processing.");
            }
        }

        #endregion Stubs

        [Fact]
        public async Task StartAsync_ShouldConsumeAndDispatchMessages()
        {
            // Arrange
            var topic = KafkaTopic.ProcessingTestTopic.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var worker = CreateWorker(
                [topic],
                handler: ConsumeResultHandler.Create());

            await ProduceAsync(topic, "test-value");

            var workerImpl = worker.ToImplementation<KafkaConsumerWorker<Null, string>>();

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await worker.StartAsync(cts.Token);
            await worker.ExecuteAsync(cts.Token);

            // Assert
            Assert.True(
                workerImpl.WorkItems.IsEmpty ||
                workerImpl.WorkItems.All(workItem => workItem.IsHandled || workItem.IsCompleted));

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ExecuteAsync_OnException_ShouldProduceToRetryTopic()
        {
            // Arrange
            var topic = KafkaTopic.FaultyProcessingTestTopic1.GetDescription();
            var retryTopic = KafkaTopic.FaultyProcessingTestTopic1Retry.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic, retryTopic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var worker = CreateWorker(
                [topic],
                enableRetryTopic: true,
                handler: FaultyConsumeResultHandler.Create());

            using var retryConsumer = CreateConsumer<byte[], KafkaMetadataMessage>(
                [retryTopic],
                deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            await ProduceAsync(topic, "test-value");

            var workerImpl = worker.ToImplementation<KafkaConsumerWorker<Null, string>>();

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await worker.StartAsync(cts.Token);
            await worker.ExecuteAsync(cts.Token);

            var retryMessage = retryConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.True(
                workerImpl.WorkItems.IsEmpty ||
                workerImpl.WorkItems.All(workItem => workItem.IsHandled || workItem.IsCompleted));

            Assert.NotNull(retryMessage);
            Assert.Equal(retryTopic, retryMessage.Topic);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public async Task ExecuteAsync_OnException_ShouldProduceToDeadLetterTopic()
        {
            // Arrange
            var topic = KafkaTopic.FaultyProcessingTestTopic2.GetDescription();
            var deadLetterTopic = KafkaTopic.FaultyProcessingTestTopic2DeadLetter.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic, deadLetterTopic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var worker = CreateWorker(
                [topic],
                enableDeadLetterTopic: true,
                handler: FaultyConsumeResultHandler.Create());

            using var deadLetterConsumer = CreateConsumer<byte[], KafkaMetadataMessage>(
                [deadLetterTopic],
                deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            await ProduceAsync(topic, "test-value");

            var workerImpl = worker.ToImplementation<KafkaConsumerWorker<Null, string>>();

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await worker.StartAsync(cts.Token);
            await worker.ExecuteAsync(cts.Token);

            var deadLetterMessage = deadLetterConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.True(
                workerImpl.WorkItems.IsEmpty ||
                workerImpl.WorkItems.All(workItem => workItem.IsHandled || workItem.IsCompleted));

            Assert.NotNull(deadLetterMessage);
            Assert.Equal(deadLetterTopic, deadLetterMessage.Topic);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        private IKafkaConsumerWorker<TKey, TValue> CreateWorker<TKey, TValue>(
            IEnumerable<string> topics = null,
            bool? enableRetryTopic = null,
            bool? enableDeadLetterTopic = null,
            IConsumeResultHandler<TKey, TValue> handler = null)
        {
            var builder = new KafkaConsumerWorkerBuilder<TKey, TValue>(
                new KafkaConsumerWorkerConfig
                {
                    EnableRetryTopic = enableRetryTopic ?? false,
                    EnableDeadLetterTopic = enableDeadLetterTopic ?? false
                })
                .WithConsumer(CreateConsumer<TKey, TValue>(topics))
                .WithConsumeResultHandler(handler)
                .WithLoggerFactory(_mockLoggerFactory.Object);

            if (enableRetryTopic ?? false)
            {
                builder.WithRetryProducer(
                    CreateProducer<byte[], KafkaMetadataMessage>(
                        serializer: CreateJsonCoreSerializer<KafkaMetadataMessage>()));
            }

            if (enableDeadLetterTopic ?? false)
            {
                builder.WithDeadLetterProducer(
                    CreateProducer<byte[], KafkaMetadataMessage>(
                        serializer: CreateJsonCoreSerializer<KafkaMetadataMessage>()));
            }

            var worker = builder.Build();

            return worker;
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
                    GroupId = "test-worker-group",
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

        private async Task ProduceAsync(string topic, string value)
        {
            await _producer.ProduceAsync(
                topic,
                new Message<Null, byte[]>
                {
                    Value = _encoding.GetBytes(value)
                });

            _producer.Flush(DefaultTimeout);
        }
    }
}
