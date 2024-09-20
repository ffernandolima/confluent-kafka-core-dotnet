using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Hosting.Internal;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Diagnostics
{
    using System.Text;

    public sealed class KafkaDiagnosticsManagerEnrichmentTests : IDisposable
    {
        private readonly Encoding _encoding;
        private readonly ActivityListener _listener;
        private readonly Dictionary<string, string> _carrier;
        private readonly IKafkaDiagnosticsManager _diagnosticsManager;

        public KafkaDiagnosticsManagerEnrichmentTests()
        {
            var options = new KafkaEnrichmentOptions
            {
                EnrichConsumptionFailure = (activity, context) =>
                {
                    activity.SetTag("custom-consumption-failure-tag", "consumption-failure-value");
                },
                EnrichConsumption = (activity, context) =>
                {
                    activity.SetTag("custom-consumption-tag", "consumption-value");
                },
                EnrichProductionFailure = (activity, context) =>
                {
                    activity.SetTag("custom-production-failure-tag", "production-failure-value");
                },
                EnrichSyncProduction = (activity, context) =>
                {
                    activity.SetTag("custom-sync-production-tag", "sync-production-value");
                },
                EnrichAsyncProduction = (activity, context) =>
                {
                    activity.SetTag("custom-async-production-tag", "async-production-value");
                },
                EnrichProcessingFailure = (activity, context) =>
                {
                    activity.SetTag("custom-processing-failure-tag", "processing-failure-value");
                },
                EnrichProcessing = (activity, context) =>
                {
                    activity.SetTag("custom-processing-tag", "processing-value");
                }
            };

            _diagnosticsManager = new KafkaDiagnosticsManager(options);
            _encoding = EncodingFactory.Instance.CreateDefault();
            _listener = KafkaActivityListener.StartListening();
            _carrier = [];
        }

        public void Dispose()
        {
            _listener?.Dispose();

            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Enrich_ShouldAddAllRelevantTagsForConsumeException()
        {
            // Arrange
            var activityName = "consumer-activity";
            var activity = _diagnosticsManager.StartConsumerActivity(activityName, _carrier);
            var consumeResult = new ConsumeResult<byte[], byte[]>
            {
                Topic = "diagnostics-test-topic",
                Partition = new Partition(1),
                Offset = new Offset(100),
                Message = new Message<byte[], byte[]>
                {
                    Value = _encoding.GetBytes("test-message")
                }
            };

            var consumeException = new ConsumeException(consumeResult, new Error(ErrorCode.Unknown));

            var consumerConfig = new KafkaConsumerConfig
            {
                GroupId = "test-group",
                BootstrapServers = "localhost:9092"
            };

            // Act
            _diagnosticsManager.Enrich(activity, consumeException, consumerConfig);

            // Assert
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ClientId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.DestinationName);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageBodySize);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Operation);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.System);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.NetworkTransport);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ServerAddress);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionType);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionMessage);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionStackTrace);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ConsumerGroup);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.DestinationPartition);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageOffset);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageTombstone);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultErrorCode);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultErrorReason);
            Assert.Contains(activity.TagObjects, tag => tag.Key == "custom-consumption-failure-tag");
        }

        [Fact]
        public void Enrich_ShouldAddAllRelevantTagsForConsumeResult()
        {
            // Arrange
            var activityName = "consumer-activity";
            var activity = _diagnosticsManager.StartConsumerActivity(activityName, _carrier);
            var consumeResult = new ConsumeResult<Null, string>
            {
                Topic = "diagnostics-test-topic",
                Partition = new Partition(1),
                Offset = new Offset(100),
                Message = new Message<Null, string>
                {
                    Value = "test-message"
                }
            };

            var consumerOptions = new KafkaConsumerOptions<Null, string>
            {
                ConsumerConfig = new KafkaConsumerConfig
                {
                    GroupId = "test-group",
                    BootstrapServers = "localhost:9092"
                },
                ValueDeserializer = Deserializers.Utf8,
                MessageIdHandler = value => value
            };

            // Act
            _diagnosticsManager.Enrich(activity, consumeResult, consumerOptions);

            // Assert
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ClientId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.DestinationName);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageBodySize);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Operation);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.System);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.NetworkTransport);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ServerAddress);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ConsumerGroup);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.DestinationPartition);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageOffset);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageTombstone);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == "custom-consumption-tag");
        }

        [Fact]
        public void Enrich_ShouldAddAllRelevantTagsForProduceException()
        {
            // Arrange
            var activityName = "producer-activity";
            var activity = _diagnosticsManager.StartProducerActivity(activityName, _carrier);

            var deliveryResult = new DeliveryResult<Null, string>
            {
                Topic = "diagnostics-test-topic",
                Partition = new Partition(1),
                Offset = new Offset(100),
                Message = new Message<Null, string>
                {
                    Value = "test-message"
                },
                Status = PersistenceStatus.NotPersisted
            };

            var produceException = new ProduceException<Null, string>(new Error(ErrorCode.Unknown), deliveryResult);

            var producerOptions = new KafkaProducerOptions<Null, string>
            {
                ProducerConfig = new KafkaProducerConfig
                {
                    BootstrapServers = "localhost:9092"
                },
                ValueSerializer = Serializers.Utf8,
                MessageIdHandler = value => value
            };

            // Act
            _diagnosticsManager.Enrich(activity, produceException, producerOptions);

            // Assert
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ClientId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.DestinationName);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageBodySize);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Operation);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.System);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.NetworkTransport);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ServerAddress);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionType);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionMessage);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionStackTrace);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.DestinationPartition);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageOffset);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageTombstone);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultErrorCode);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultErrorReason);
            Assert.Contains(activity.TagObjects, tag => tag.Key == "custom-production-failure-tag");
        }

        [Fact]
        public void Enrich_ShouldAddAllRelevantTagsForDeliveryReport()
        {
            // Arrange
            var activityName = "producer-activity";
            var activity = _diagnosticsManager.StartProducerActivity(activityName, _carrier);
            var deliveryReport = new DeliveryReport<Null, string>
            {
                Topic = "diagnostics-test-topic",
                Partition = new Partition(1),
                Offset = new Offset(100),
                Message = new Message<Null, string>
                {
                    Value = "test-message"
                },
                Status = PersistenceStatus.Persisted
            };

            var producerOptions = new KafkaProducerOptions<Null, string>
            {
                ProducerConfig = new KafkaProducerConfig
                {
                    BootstrapServers = "localhost:9092"
                },
                ValueSerializer = Serializers.Utf8,
                MessageIdHandler = value => value
            };

            // Act
            _diagnosticsManager.Enrich(activity, deliveryReport, producerOptions);

            // Assert
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ClientId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.DestinationName);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageBodySize);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Operation);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.System);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.NetworkTransport);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ServerAddress);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.DestinationPartition);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageOffset);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageTombstone);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == "custom-sync-production-tag");
        }

        [Fact]
        public void Enrich_ShouldAddAllRelevantTagsForDeliveryResult()
        {
            // Arrange
            var activityName = "producer-activity";
            var activity = _diagnosticsManager.StartProducerActivity(activityName, _carrier);
            var deliveryResult = new DeliveryResult<Null, string>
            {
                Topic = "diagnostics-test-topic",
                Partition = new Partition(1),
                Offset = new Offset(100),
                Message = new Message<Null, string>
                {
                    Value = "test-message"
                },
                Status = PersistenceStatus.Persisted
            };

            var producerOptions = new KafkaProducerOptions<Null, string>
            {
                ProducerConfig = new KafkaProducerConfig
                {
                    BootstrapServers = "localhost:9092"
                },
                ValueSerializer = Serializers.Utf8,
                MessageIdHandler = value => value
            };

            // Act
            _diagnosticsManager.Enrich(activity, deliveryResult, producerOptions);

            // Assert
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ClientId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.DestinationName);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageBodySize);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Operation);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.System);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.NetworkTransport);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ServerAddress);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.DestinationPartition);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageOffset);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageTombstone);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == "custom-async-production-tag");
        }

        [Fact]
        public void Enrich_ShouldAddAllRelevantTagsForProcessingException()
        {
            // Arrange
            var activityName = "consumer-activity";
            var activity = _diagnosticsManager.StartConsumerActivity(activityName, _carrier);
            var consumeResult = new ConsumeResult<Null, string>
            {
                Topic = "diagnostics-test-topic",
                Partition = new Partition(1),
                Offset = new Offset(100),
                Message = new Message<Null, string>
                {
                    Value = "test-message"
                }
            };

            var exception = new Exception("Test exception");

            var consumer = new KafkaConsumerBuilder<Null, string>(
                new KafkaConsumerConfig
                {
                    GroupId = "test-group",
                    BootstrapServers = "localhost:9092"
                })
               .WithMessageIdHandler(value => value)
               .Build();

            var workerOptions = new KafkaConsumerWorkerOptions<Null, string>
            {
                Consumer = consumer,
                WorkerConfig = new KafkaConsumerWorkerConfig()
            };

            // Act
            _diagnosticsManager.Enrich(activity, exception, consumeResult, workerOptions);

            // Assert
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ClientId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.DestinationName);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageBodySize);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Operation);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.System);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.NetworkTransport);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ServerAddress);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionType);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionMessage);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ExceptionStackTrace);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ConsumerGroup);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.DestinationPartition);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageOffset);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageTombstone);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ProcessingIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == "custom-processing-failure-tag");
        }

        [Fact]
        public void Enrich_ShouldAddAllRelevantTagsForProcessing()
        {
            // Arrange
            var activityName = "consumer-activity";
            var activity = _diagnosticsManager.StartConsumerActivity(activityName, _carrier);
            var consumeResult = new ConsumeResult<Null, string>
            {
                Topic = "diagnostics-test-topic",
                Partition = new Partition(1),
                Offset = new Offset(100),
                Message = new Message<Null, string>
                {
                    Value = "test-message"
                }
            };

            var consumer = new KafkaConsumerBuilder<Null, string>(
                new KafkaConsumerConfig
                {
                    GroupId = "test-group",
                    BootstrapServers = "localhost:9092"
                })
               .WithMessageIdHandler(value => value)
               .Build();

            var workerOptions = new KafkaConsumerWorkerOptions<Null, string>
            {
                Consumer = consumer,
                WorkerConfig = new KafkaConsumerWorkerConfig()
            };

            // Act
            _diagnosticsManager.Enrich(activity, consumeResult, workerOptions);

            // Assert
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ClientId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.DestinationName);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageBodySize);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.MessageId);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Operation);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.System);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.NetworkTransport);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.ServerAddress);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ConsumerGroup);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.DestinationPartition);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageOffset);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.MessageTombstone);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ResultIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == SemanticConventions.Messaging.Kafka.ProcessingIsError);
            Assert.Contains(activity.TagObjects, tag => tag.Key == "custom-processing-tag");
        }
    }
}
