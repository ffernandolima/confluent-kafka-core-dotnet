using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Serialization.Internal;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaActivityEnricher : IKafkaActivityEnricher
    {
        private static readonly ConcurrentDictionary<Type, object> Serializers = [];

        private readonly KafkaEnrichmentOptions _options;

        public KafkaActivityEnricher(KafkaEnrichmentOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Enrich(Activity activity, ConsumeException consumeException, IKafkaConsumerConfig consumerConfig)
        {
            if (activity is null)
            {
                return;
            }

            if (consumeException is null)
            {
                throw new ArgumentNullException(nameof(consumeException));
            }

            if (consumerConfig is null)
            {
                throw new ArgumentNullException(nameof(consumerConfig));
            }

            var consumerRecord = consumeException.ConsumerRecord ?? throw new InvalidOperationException("Consumer record cannot be null.");

            using var builder = new KafkaActivityAttributesBuilder()
                .WithBootstrapServers(consumerConfig.BootstrapServers)
                .WithGroupId(consumerConfig.GroupId)
                .WithTopic(consumerRecord.Topic)
                .WithPartition(consumerRecord.Partition)
                .WithOffset(consumerRecord.Offset)
                .WithMessageKey(consumerRecord.Message!.Key)
                .WithMessageValue(consumerRecord.Message!.Value)
                .WithMessageBody(consumerRecord.Message!.Value)
                .WithOperation(OperationNames.ReceiveOperation)
                .WithError(consumeException.Error)
                .WithException(consumeException);

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);

            _options.EnrichConsumptionFailure?.Invoke(activity,
                new KafkaConsumptionFailureEnrichmentContext
                {
                    Topic = consumerRecord.Topic,
                    Partition = consumerRecord.Partition,
                    Offset = consumerRecord.Offset,
                    Key = consumerRecord.Message!.Key,
                    Value = consumerRecord.Message!.Value,
                    Timestamp = consumerRecord.Message!.Timestamp,
                    Headers = consumerRecord.Message!.Headers,
                    ConsumerConfig = consumerConfig,
                    Error = consumeException.Error,
                    Exception = consumeException
                });
        }

        public void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithBootstrapServers(options.ConsumerConfig!.BootstrapServers)
                .WithGroupId(options.ConsumerConfig!.GroupId)
                .WithTopic(consumeResult.Topic)
                .WithPartition(consumeResult.Partition)
                .WithOffset(consumeResult.Offset)
                .WithMessageKey(consumeResult.Message!.Key)
                .WithMessageValue(consumeResult.Message!.Value)
                .WithMessageId(consumeResult.Message!.GetId(options.MessageIdHandler))
                .WithMessageBody(
                    GetMessageBody(
                        consumeResult.Message!.Value,
                        options.ValueDeserializer,
                        () => new(MessageComponentType.Value, consumeResult.Topic, consumeResult.Message!.Headers)))
                .WithOperation(OperationNames.ReceiveOperation);

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);

            _options.EnrichConsumption?.Invoke(activity,
                new KafkaConsumptionEnrichmentContext
                {
                    Topic = consumeResult.Topic,
                    Partition = consumeResult.Partition,
                    Offset = consumeResult.Offset,
                    Key = consumeResult.Message!.Key,
                    Value = consumeResult.Message!.Value,
                    Timestamp = consumeResult.Message!.Timestamp,
                    Headers = consumeResult.Message!.Headers,
                    ConsumerConfig = options.ConsumerConfig
                });
        }

        public void Enrich<TKey, TValue>(Activity activity, ProduceException<TKey, TValue> produceException, IKafkaProducerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (produceException is null)
            {
                throw new ArgumentNullException(nameof(produceException));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var deliveryResult = produceException.DeliveryResult ?? throw new InvalidOperationException("Delivery result cannot be null.");

            using var builder = new KafkaActivityAttributesBuilder()
                .WithBootstrapServers(options.ProducerConfig!.BootstrapServers)
                .WithTopic(deliveryResult.Topic)
                .WithPartition(deliveryResult.Partition)
                .WithOffset(deliveryResult.Offset)
                .WithMessageKey(deliveryResult.Message!.Key)
                .WithMessageValue(deliveryResult.Message!.Value)
                .WithMessageId(deliveryResult.Message!.GetId(options.MessageIdHandler))
                .WithMessageBody(
                    GetMessageBody(
                        deliveryResult.Message!.Value,
                        options.ValueSerializer,
                        () => new(MessageComponentType.Value, deliveryResult.Topic, deliveryResult.Message!.Headers)))
                .WithOperation(OperationNames.PublishOperation)
                .WithError(produceException.Error)
                .WithException(produceException);

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);

            _options.EnrichProductionFailure?.Invoke(activity,
                new KafkaProductionFailureEnrichmentContext
                {
                    Topic = deliveryResult.Topic,
                    Partition = deliveryResult.Partition,
                    Offset = deliveryResult.Offset,
                    Key = deliveryResult.Message!.Key,
                    Value = deliveryResult.Message!.Value,
                    Timestamp = deliveryResult.Message!.Timestamp,
                    Headers = deliveryResult.Message!.Headers,
                    Status = deliveryResult.Status,
                    ProducerConfig = options.ProducerConfig,
                    Error = produceException.Error,
                    Exception = produceException
                });
        }

        public void Enrich<TKey, TValue>(Activity activity, DeliveryReport<TKey, TValue> deliveryReport, IKafkaProducerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (deliveryReport is null)
            {
                throw new ArgumentNullException(nameof(deliveryReport));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithBootstrapServers(options.ProducerConfig!.BootstrapServers)
                .WithTopic(deliveryReport.Topic)
                .WithPartition(deliveryReport.Partition)
                .WithOffset(deliveryReport.Offset)
                .WithMessageKey(deliveryReport.Message!.Key)
                .WithMessageValue(deliveryReport.Message!.Value)
                .WithMessageId(deliveryReport.Message!.GetId(options.MessageIdHandler))
                .WithMessageBody(
                    GetMessageBody(
                        deliveryReport.Message!.Value,
                        options.ValueSerializer,
                        () => new(MessageComponentType.Value, deliveryReport.Topic, deliveryReport.Message!.Headers)))
                .WithOperation(OperationNames.PublishOperation)
                .WithError(deliveryReport.Error);

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);

            _options.EnrichSyncProduction?.Invoke(activity,
                new KafkaSyncProductionEnrichmentContext
                {
                    Topic = deliveryReport.Topic,
                    Partition = deliveryReport.Partition,
                    Offset = deliveryReport.Offset,
                    Key = deliveryReport.Message!.Key,
                    Value = deliveryReport.Message!.Value,
                    Timestamp = deliveryReport.Message!.Timestamp,
                    Headers = deliveryReport.Message!.Headers,
                    Status = deliveryReport.Status,
                    ProducerConfig = options.ProducerConfig,
                    Error = deliveryReport.Error
                });
        }

        public void Enrich<TKey, TValue>(Activity activity, DeliveryResult<TKey, TValue> deliveryResult, IKafkaProducerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (deliveryResult is null)
            {
                throw new ArgumentNullException(nameof(deliveryResult));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithBootstrapServers(options.ProducerConfig!.BootstrapServers)
                .WithTopic(deliveryResult.Topic)
                .WithPartition(deliveryResult.Partition)
                .WithOffset(deliveryResult.Offset)
                .WithMessageKey(deliveryResult.Message!.Key)
                .WithMessageValue(deliveryResult.Message!.Value)
                .WithMessageId(deliveryResult.Message!.GetId(options.MessageIdHandler))
                .WithMessageBody(
                    GetMessageBody(
                        deliveryResult.Message!.Value,
                        options.ValueSerializer,
                        () => new(MessageComponentType.Value, deliveryResult.Topic, deliveryResult.Message!.Headers)))
                .WithOperation(OperationNames.PublishOperation);

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);

            _options.EnrichAsyncProduction?.Invoke(activity,
                new KafkaAsyncProductionEnrichmentContext
                {
                    Topic = deliveryResult.Topic,
                    Partition = deliveryResult.Partition,
                    Offset = deliveryResult.Offset,
                    Key = deliveryResult.Message!.Key,
                    Value = deliveryResult.Message!.Value,
                    Timestamp = deliveryResult.Message!.Timestamp,
                    Headers = deliveryResult.Message!.Headers,
                    Status = deliveryResult.Status,
                    ProducerConfig = options.ProducerConfig
                });
        }

        public void Enrich<TKey, TValue>(Activity activity, Exception exception, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerWorkerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithBootstrapServers(options.Consumer!.Options!.ConsumerConfig!.BootstrapServers)
                .WithGroupId(options.Consumer!.Options!.ConsumerConfig!.GroupId)
                .WithTopic(consumeResult.Topic)
                .WithPartition(consumeResult.Partition)
                .WithOffset(consumeResult.Offset)
                .WithMessageKey(consumeResult.Message!.Key)
                .WithMessageValue(consumeResult.Message!.Value)
                .WithMessageId(consumeResult.Message!.GetId(options.Consumer!.Options!.MessageIdHandler))
                .WithMessageBody(
                    GetMessageBody(
                        consumeResult.Message!.Value,
                        options.Consumer!.Options!.ValueDeserializer,
                        () => new(MessageComponentType.Value, consumeResult.Topic, consumeResult.Message!.Headers)))
                .WithOperation(OperationNames.ProcessOperation)
                .WithException(exception);

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);

            activity.SetTag(SemanticConventions.Messaging.Kafka.ProcessingIsError, true);

            _options.EnrichProcessingFailure?.Invoke(activity,
                new KafkaProcessingFailureEnrichmentContext
                {
                    Topic = consumeResult.Topic,
                    Partition = consumeResult.Partition,
                    Offset = consumeResult.Offset,
                    Key = consumeResult.Message!.Key,
                    Value = consumeResult.Message!.Value,
                    Timestamp = consumeResult.Message!.Timestamp,
                    Headers = consumeResult.Message!.Headers,
                    ConsumerConfig = options.Consumer!.Options!.ConsumerConfig,
                    WorkerConfig = options.WorkerConfig,
                    Exception = exception
                });
        }

        public void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerWorkerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithBootstrapServers(options.Consumer!.Options!.ConsumerConfig!.BootstrapServers)
                .WithGroupId(options.Consumer!.Options!.ConsumerConfig!.GroupId)
                .WithTopic(consumeResult.Topic)
                .WithPartition(consumeResult.Partition)
                .WithOffset(consumeResult.Offset)
                .WithMessageKey(consumeResult.Message!.Key)
                .WithMessageValue(consumeResult.Message!.Value)
                .WithMessageId(consumeResult.Message!.GetId(options.Consumer!.Options!.MessageIdHandler))
                .WithMessageBody(
                    GetMessageBody(
                        consumeResult.Message!.Value,
                        options.Consumer!.Options!.ValueDeserializer,
                        () => new(MessageComponentType.Value, consumeResult.Topic, consumeResult.Message!.Headers)))
                .WithOperation(OperationNames.ProcessOperation);

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);

            activity.SetTag(SemanticConventions.Messaging.Kafka.ProcessingIsError, false);

            _options.EnrichProcessing?.Invoke(activity,
                new KafkaProcessingEnrichmentContext
                {
                    Topic = consumeResult.Topic,
                    Partition = consumeResult.Partition,
                    Offset = consumeResult.Offset,
                    Key = consumeResult.Message!.Key,
                    Value = consumeResult.Message!.Value,
                    Timestamp = consumeResult.Message!.Timestamp,
                    Headers = consumeResult.Message!.Headers,
                    ConsumerConfig = options.Consumer!.Options!.ConsumerConfig,
                    WorkerConfig = options.WorkerConfig
                });
        }

        private static byte[] GetMessageBody<TValue>(TValue messageValue, object configuredSerializer, Func<SerializationContext> contextFactory)
        {
            if (messageValue is null)
            {
                return null;
            }

            if (messageValue is byte[] messageBody)
            {
                return messageBody;
            }

            try
            {
                var serializer = (ISerializer<TValue>)Serializers.GetOrAdd(
                    typeof(TValue),
                    key => configuredSerializer.ToSerializer<TValue>());

                messageBody = serializer?.Serialize(messageValue, contextFactory.Invoke());
            }
            catch
            {
                messageBody = null;
            }

            return messageBody;
        }

        private static void EnrichInternal(Activity activity, KafkaActivityAttributes attributes)
        {
            activity.SetTag(SemanticConventions.Messaging.System, attributes.System);
            activity.SetTag(SemanticConventions.Messaging.Operation, attributes.Operation);
            activity.SetTag(SemanticConventions.Messaging.ClientId, attributes.ClientId);

            if (attributes.MessageId is not null)
            {
                activity.SetTag(SemanticConventions.Messaging.MessageId, attributes.MessageId);
            }

            activity.SetTag(SemanticConventions.Messaging.Kafka.MessageOffset, attributes.MessageOffset);

            if (!string.IsNullOrWhiteSpace(attributes.MessageKey))
            {
                activity.SetTag(SemanticConventions.Messaging.Kafka.MessageKey, attributes.MessageKey);
            }

            if (attributes.MessageBodySize.HasValue)
            {
                activity.SetTag(SemanticConventions.Messaging.MessageBodySize, attributes.MessageBodySize);
            }

            activity.SetTag(SemanticConventions.Messaging.Kafka.MessageTombstone, attributes.MessageTombstone);

            if (!string.IsNullOrWhiteSpace(attributes.ConsumerGroup))
            {
                activity.SetTag(SemanticConventions.Messaging.Kafka.ConsumerGroup, attributes.ConsumerGroup);
            }

            if (!string.IsNullOrWhiteSpace(attributes.DestinationName))
            {
                activity.SetTag(SemanticConventions.Messaging.DestinationName, attributes.DestinationName);
            }

            if (attributes.DestinationPartition.HasValue)
            {
                activity.SetTag(SemanticConventions.Messaging.Kafka.DestinationPartition, attributes.DestinationPartition);
            }

            activity.SetTag(SemanticConventions.Messaging.Kafka.ResultIsError, attributes.ResultIsError);

            if (attributes.ResultIsError)
            {
                activity.SetTag(SemanticConventions.Messaging.Kafka.ResultErrorCode, attributes.ResultErrorCode);
                activity.SetTag(SemanticConventions.Messaging.Kafka.ResultErrorReason, attributes.ResultErrorReason);
            }

            activity.SetTag(SemanticConventions.Messaging.NetworkTransport, attributes.NetworkTransport);

            if (!string.IsNullOrWhiteSpace(attributes.ServerAddress))
            {
                activity.SetTag(SemanticConventions.Messaging.ServerAddress, attributes.ServerAddress);
            }

            if (!string.IsNullOrWhiteSpace(attributes.ExceptionType))
            {
                activity.SetTag(SemanticConventions.Messaging.ExceptionType, attributes.ExceptionType);
            }

            if (!string.IsNullOrWhiteSpace(attributes.ExceptionMessage))
            {
                activity.SetTag(SemanticConventions.Messaging.ExceptionMessage, attributes.ExceptionMessage);
            }

            if (!string.IsNullOrWhiteSpace(attributes.ExceptionStackTrace))
            {
                activity.SetTag(SemanticConventions.Messaging.ExceptionStackTrace, attributes.ExceptionStackTrace);
            }
        }
    }
}
