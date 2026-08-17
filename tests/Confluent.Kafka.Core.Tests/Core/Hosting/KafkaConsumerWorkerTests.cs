using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics.Internal;
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

    public sealed class KafkaConsumerWorkerTests : IClassFixture<KafkaConsumerWorkerTests.TopicFixture>, IDisposable
    {
        private const string BootstrapServers = "localhost:9092";

        private static readonly int DefaultRetryCount = 3;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        // ExecuteAsync runs until cancellation, so every test spends this in full. Keep it tight.
        private static readonly TimeSpan DefaultDelay = TimeSpan.FromSeconds(3);

        // Flush returns the number of messages still in flight; delivery can take seconds under load.
        private static readonly TimeSpan FlushTimeout = TimeSpan.FromSeconds(30);

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly Encoding _encoding;
        private readonly IKafkaProducer<Null, byte[]> _producer;

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
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        #region Stubs

        public sealed class TopicFixture : KafkaTopicFixture
        {
            public TopicFixture()
                : base(
                    BootstrapServers,
                    Enum.GetValues<KafkaTopic>()
                        .Select(value => value.GetDescription()))
            { }
        }

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

            [Description("traced-processing-test-topic-1")]
            TracedProcessingTestTopic1,

            [Description($"traced-processing-test-topic-1{KafkaRetryConstants.RetryTopicSuffix}")]
            TracedProcessingTestTopic1Retry,

            [Description("traced-processing-test-topic-2")]
            TracedProcessingTestTopic2,

            [Description($"traced-processing-test-topic-2{KafkaProducerConstants.DeadLetterTopicSuffix}")]
            TracedProcessingTestTopic2DeadLetter,
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

            using var meterListener = new KafkaMeterListener();

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

            var processed = Assert.Single(
                meterListener.ByInstrument(SemanticConventions.Metrics.ProcessDuration),
                measurement => Equals(measurement.GetTag(SemanticConventions.Messaging.DestinationName), topic));

            Assert.Equal("s", processed.Unit);
            Assert.Equal("process", processed.GetTag(SemanticConventions.Messaging.OperationName));
            Assert.Equal("process", processed.GetTag(SemanticConventions.Messaging.OperationType));
            Assert.False(processed.HasTag(SemanticConventions.Messaging.ErrorType));

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

        [Fact]
        public async Task ExecuteAsync_OnException_ShouldPreserveTraceAcrossRetryTopic()
        {
            // Arrange
            var topic = KafkaTopic.TracedProcessingTestTopic1.GetDescription();
            var retryTopic = KafkaTopic.TracedProcessingTestTopic1Retry.GetDescription();

            var retryActivities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(retryTopic))
                {
                    retryActivities.Add(activity);
                }
            });

            using var worker = CreateWorker(
                [topic],
                enableRetryTopic: true,
                handler: FaultyConsumeResultHandler.Create());

            using var retryConsumer = CreateConsumer<byte[], KafkaMetadataMessage>(
                [retryTopic],
                deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            using var rootActivity = new Activity("trace-continuity-root").Start();

            var expectedTraceId = rootActivity.TraceId;

            await ProduceAsync(topic, "test-value");

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await worker.StartAsync(cts.Token);
            await worker.ExecuteAsync(cts.Token);

            var retryMessage = retryConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(retryMessage);

            AssertTraceFlowsThroughTheMessage(retryActivities, expectedTraceId);
        }

        [Fact]
        public async Task ExecuteAsync_OnException_ShouldPreserveTraceAcrossDeadLetterTopic()
        {
            // Arrange
            var topic = KafkaTopic.TracedProcessingTestTopic2.GetDescription();
            var deadLetterTopic = KafkaTopic.TracedProcessingTestTopic2DeadLetter.GetDescription();

            var deadLetterActivities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(deadLetterTopic))
                {
                    deadLetterActivities.Add(activity);
                }
            });

            using var worker = CreateWorker(
                [topic],
                enableDeadLetterTopic: true,
                handler: FaultyConsumeResultHandler.Create());

            using var deadLetterConsumer = CreateConsumer<byte[], KafkaMetadataMessage>(
                [deadLetterTopic],
                deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            using var rootActivity = new Activity("trace-continuity-root").Start();

            var expectedTraceId = rootActivity.TraceId;

            await ProduceAsync(topic, "test-value");

            // Act
            var cts = new CancellationTokenSource(DefaultDelay);

            await worker.StartAsync(cts.Token);
            await worker.ExecuteAsync(cts.Token);

            var deadLetterMessage = deadLetterConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(deadLetterMessage);

            AssertTraceFlowsThroughTheMessage(deadLetterActivities, expectedTraceId);
        }

        /// <summary>
        /// The producer and the consumer of the downstream topic are separate clients, so the only
        /// way the consume span can be a child of the produce span is via the trace context carried
        /// in the message headers. Asserting the trace id alone would not prove that: in a single
        /// process the ambient Activity.Current preserves it even when propagation is broken.
        /// </summary>
        private static void AssertTraceFlowsThroughTheMessage(List<Activity> activities, ActivityTraceId expectedTraceId)
        {
            Assert.NotEmpty(activities);
            Assert.All(activities, activity => Assert.Equal(expectedTraceId, activity.TraceId));

            var produceActivity = Assert.Single(activities, activity => activity.Kind == ActivityKind.Producer);

            Assert.Contains(activities, activity => activity.Kind == ActivityKind.Client && activity.ParentSpanId == produceActivity.SpanId);
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

            Assert.Equal(0, _producer.Flush(FlushTimeout));
        }
    }
}
