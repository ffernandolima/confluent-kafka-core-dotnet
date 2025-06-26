using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
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

namespace Confluent.Kafka.Core.Tests.Core.Consumer
{
    using System.Text;

    public sealed class KafkaConsumerTests : IAsyncLifetime
    {
        private const string BootstrapServers = "localhost:9092";

        private static readonly int DefaultRetryCount = 5;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        private readonly Encoding _encoding;
        private readonly IKafkaProducer<Null, byte[]> _producer;

        private readonly KafkaTopicFixture _kafkaTopicFixture;

        public KafkaConsumerTests()
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
            [Description("consumption-test-topic")]
            ConsumptionTestTopic,

            [Description("consumption-multiple-test-topic-1")]
            ConsumptionMultipleTestTopic1,

            [Description("consumption-multiple-test-topic-2")]
            ConsumptionMultipleTestTopic2,

            [Description("consumption-empty-test-topic")]
            ConsumptionEmptyTestTopic,

            [Description("consumption-canceled-test-topic")]
            ConsumptionCanceledTestTopic,

            [Description("batch-consumption-test-topic")]
            BatchConsumptionTestTopic,

            [Description("partial-batch-consumption-test-topic")]
            PartialBatchConsumptionTestTopic,

            [Description("delayed-batch-consumption-test-topic")]
            DelayedBatchConsumptionTestTopic,

            [Description("wrong-format-consumption-test-topic")]
            WrongFormatConsumptionTestTopic,

            [Description("wrong-format-batch-consumption-test-topic")]
            WrongFormatBatchConsumptionTestTopic,

            [Description($"wrong-format-consumption-test-topic{KafkaProducerConstants.DeadLetterTopicSuffix}")]
            WrongFormatConsumptionTestTopicDeadLetter,

            [Description($"wrong-format-batch-consumption-test-topic{KafkaProducerConstants.DeadLetterTopicSuffix}")]
            WrongFormatBatchConsumptionTestTopicDeadLetter
        }

        public class Message
        {
            public int Id { get; set; }
            public string Content { get; set; }
        }

        #endregion Stubs

        [Fact]
        public async Task Consume_FromSingleTopic_ShouldConsumeMessageSuccessfully()
        {
            // Arrange
            var topic = KafkaTopic.ConsumptionTestTopic.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, string>([topic]);

            await ProduceAsync(topic, "test-value");

            // Act
            var result = consumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(topic, result.Topic);
            Assert.Equal("test-value", result.Message.Value);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task Consume_FromMultipleTopics_ShouldConsumeMessagesSuccessfully()
        {
            // Arrange
            var topics = new[]
            {
                KafkaTopic.ConsumptionMultipleTestTopic1.GetDescription(),
                KafkaTopic.ConsumptionMultipleTestTopic2.GetDescription()
            };

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topics) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, string>(topics);

            await ProduceAsync(topics[0], "test-value");
            await ProduceAsync(topics[1], "test-value");

            // Act
            var result1 = consumer.Consume(DefaultTimeout, DefaultRetryCount);
            var result2 = consumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);

            Assert.Contains(result1.Topic, topics);
            Assert.Contains(result2.Topic, topics);

            Assert.Equal("test-value", result1.Message.Value);
            Assert.Equal("test-value", result2.Message.Value);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Consume_FromEmptyTopic_ShouldReturnNullOrTimeout()
        {
            // Arrange
            var topic = KafkaTopic.ConsumptionEmptyTestTopic.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, string>([topic]);

            // Act
            var result = consumer.Consume();

            // Assert
            Assert.Null(result);

            Assert.Empty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void Consume_WithCancellationToken_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var topic = KafkaTopic.ConsumptionCanceledTestTopic.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, string>([topic]);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            Assert.Throws<OperationCanceledException>(() => consumer.Consume(cts.Token));

            Assert.Empty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ConsumeBatch_WithBatchSize_ShouldLimit()
        {
            // Arrange
            var topic = KafkaTopic.BatchConsumptionTestTopic.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, string>(
                [topic],
                defaultBatchSize: 5);

            for (var i = 0; i < 10; i++)
            {
                await ProduceAsync(topic, $"test-value-{i}");
            }

            // Act
            var results = consumer.ConsumeBatch(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(5, results.Count());

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task ConsumeBatch_OnTimeout_ShouldReturnPartialBatch()
        {
            // Arrange
            var topic = KafkaTopic.PartialBatchConsumptionTestTopic.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, string>([topic]);

            await ProduceAsync(topic, "test-value");

            // Act
            var results = consumer.ConsumeBatch(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal("test-value", results.ElementAt(0)!.Message!.Value);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void ConsumeBatch_DelayedConsumption_ShouldHandle()
        {
            // Arrange
            var topic = KafkaTopic.DelayedBatchConsumptionTestTopic.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, string>(
                [topic],
                defaultTimeout: TimeSpan.FromSeconds(3));

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await ProduceAsync(topic, "delayed-value");
            });

            // Act
            var results = consumer.ConsumeBatch(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal("delayed-value", results.ElementAt(0)!.Message!.Value);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task Consume_OnConsumeException_ShouldProduceDeadLetter()
        {
            // Arrange
            var topic = KafkaTopic.WrongFormatConsumptionTestTopic.GetDescription();
            var deadLetterTopic = KafkaTopic.WrongFormatConsumptionTestTopicDeadLetter.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic, deadLetterTopic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, Message>(
                [topic],
                enableDeadLetterTopic: true,
                deserializer: CreateJsonCoreSerializer<Message>());

            using var deadLetterConsumer = CreateConsumer<byte[], KafkaMetadataMessage>(
                [deadLetterTopic],
                deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            await ProduceAsync(topic, "faulty-message");

            // Act
            Assert.Throws<ConsumeException>(() => consumer.Consume(DefaultTimeout, DefaultRetryCount));

            consumer.Options!.DeadLetterProducer!.Flush(DefaultTimeout);

            var deadLetterMessage = deadLetterConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(deadLetterMessage);
            Assert.Equal(deadLetterTopic, deadLetterMessage.Topic);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public async Task ConsumeBatch_OnConsumeException_ShouldProduceDeadLetterMessage()
        {
            // Arrange
            var topic = KafkaTopic.WrongFormatBatchConsumptionTestTopic.GetDescription();
            var deadLetterTopic = KafkaTopic.WrongFormatBatchConsumptionTestTopicDeadLetter.GetDescription();

            var activities = new List<Activity>();
            using var listener = KafkaActivityListener.StartListening(activity =>
            {
                if (activity.HasTopic(topic, deadLetterTopic) && activity.IsConsumerKind())
                {
                    activities.Add(activity);
                }
            });

            using var consumer = CreateConsumer<Null, Message>(
                [topic],
                enableDeadLetterTopic: true,
                deserializer: CreateJsonCoreSerializer<Message>());

            using var deadLetterConsumer = CreateConsumer<byte[], KafkaMetadataMessage>(
                [deadLetterTopic],
                deserializer: CreateJsonCoreSerializer<KafkaMetadataMessage>());

            await ProduceAsync(topic, "faulty-message");

            // Act
            Assert.Throws<ConsumeException>(() => consumer.ConsumeBatch(DefaultTimeout, DefaultRetryCount));

            consumer.Options!.DeadLetterProducer!.Flush(DefaultTimeout);

            var deadLetterMessage = deadLetterConsumer.Consume(DefaultTimeout, DefaultRetryCount);

            // Assert
            Assert.NotNull(deadLetterMessage);
            Assert.Equal(deadLetterTopic, deadLetterMessage.Topic);

            Assert.NotEmpty(activities);

            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        private IKafkaProducer<TKey, TValue> CreateProducer<TKey, TValue>(
            TimeSpan? defaultTimeout = null,
            bool? pollAfterProducing = null,
            ISerializer<TValue> serializer = null)
        {
            var producer = new KafkaProducerBuilder<TKey, TValue>(
                new KafkaProducerConfig
                {
                    BootstrapServers = BootstrapServers,
                    DefaultTimeout = defaultTimeout ?? DefaultTimeout,
                    PollAfterProducing = pollAfterProducing ?? false,
                    EnableDiagnostics = false
                })
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .WithValueSerializer(serializer)
                .Build();

            return producer;
        }

        private IKafkaConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(
            IEnumerable<string> topics = null,
            bool? enableDeadLetterTopic = null,
            TimeSpan? defaultTimeout = null,
            int? defaultBatchSize = null,
            IDeserializer<TValue> deserializer = null)
        {
            var builder = new KafkaConsumerBuilder<TKey, TValue>(
                new KafkaConsumerConfig
                {
                    BootstrapServers = BootstrapServers,
                    GroupId = "test-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    PartitionAssignments = topics?.Select(topic => new TopicPartition(topic, new Partition(0))),
                    EnableDeadLetterTopic = enableDeadLetterTopic ?? false,
                    DefaultTimeout = defaultTimeout ?? DefaultTimeout,
                    DefaultBatchSize = defaultBatchSize ?? 100
                })
                .WithLoggerFactory(_mockLoggerFactory.Object)
                .WithValueDeserializer(deserializer);

            if (enableDeadLetterTopic ?? false)
            {
                builder.WithDeadLetterProducer(
                    CreateProducer<byte[], KafkaMetadataMessage>(
                        pollAfterProducing: true,
                        serializer: CreateJsonCoreSerializer<KafkaMetadataMessage>()));
            }

            var consumer = builder.Build();

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
