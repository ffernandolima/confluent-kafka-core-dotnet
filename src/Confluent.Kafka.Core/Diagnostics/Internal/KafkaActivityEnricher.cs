using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Serialization.Internal;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaActivityEnricher : ActivityEnricherBase
    {
        private static readonly ConcurrentDictionary<Type, object> Serializers = [];

        public override void Enrich(Activity activity, ConsumeException consumeException, IConsumerConfig consumerConfig)
        {
            if (activity is null)
            {
                return;
            }

            if (consumeException is null)
            {
                throw new ArgumentNullException(nameof(consumeException), $"{nameof(consumeException)} cannot be null.");
            }

            if (consumerConfig is null)
            {
                throw new ArgumentNullException(nameof(consumerConfig), $"{nameof(consumerConfig)} cannot be null.");
            }

            var consumerRecord = consumeException.ConsumerRecord ?? throw new InvalidOperationException("Consumer record cannot be null.");

            using var builder = new KafkaActivityAttributesBuilder()
                .WithException(consumeException)
                .WithError(consumeException.Error)
                .WithTopic(consumerRecord.Topic)
                .WithOffset(consumerRecord.Offset)
                .WithPartition(consumerRecord.Partition)
                .WithOperation(OperationNames.ReceiveOperation)
                .WithGroupId(consumerConfig.GroupId)
                .WithBootstrapServers(consumerConfig.BootstrapServers);

            if (consumerRecord.Message is not null)
            {
                builder.WithMessageKey(consumerRecord.Message.Key)
                       .WithMessageValue(consumerRecord.Message.Value)
                       .WithMessageBody(consumerRecord.Message.Value);
            }

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);
        }

        public override void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult), $"{nameof(consumeResult)} cannot be null.");
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithTopic(consumeResult.Topic)
                .WithOffset(consumeResult.Offset)
                .WithPartition(consumeResult.Partition)
                .WithOperation(OperationNames.ReceiveOperation)
                .WithGroupId(options.ConsumerConfig!.GroupId)
                .WithBootstrapServers(options.ConsumerConfig!.BootstrapServers);

            if (consumeResult.Message is not null)
            {
                builder.WithMessageId(options.MessageIdHandler?.Invoke(consumeResult.Message.Value))
                       .WithMessageKey(consumeResult.Message.Key)
                       .WithMessageValue(consumeResult.Message.Value)
                       .WithMessageBody(
                            GetMessageBody(
                                consumeResult.Message.Value,
                                options.ValueDeserializer,
                                () => new(MessageComponentType.Value, consumeResult.Topic, consumeResult.Message.Headers)));
            }

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);
        }

        public override void Enrich<TKey, TValue>(Activity activity, ProduceException<TKey, TValue> produceException, IKafkaProducerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (produceException is null)
            {
                throw new ArgumentNullException(nameof(produceException), $"{nameof(produceException)} cannot be null.");
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            var deliveryResult = produceException.DeliveryResult ?? throw new InvalidOperationException("Delivery result cannot be null.");

            using var builder = new KafkaActivityAttributesBuilder()
                .WithException(produceException)
                .WithError(produceException.Error)
                .WithTopic(deliveryResult.Topic)
                .WithOffset(deliveryResult.Offset)
                .WithPartition(deliveryResult.Partition)
                .WithOperation(OperationNames.PublishOperation)
                .WithBootstrapServers(options.ProducerConfig!.BootstrapServers);

            if (deliveryResult.Message is not null)
            {
                builder.WithMessageId(options.MessageIdHandler?.Invoke(deliveryResult.Message.Value))
                       .WithMessageKey(deliveryResult.Message.Key)
                       .WithMessageValue(deliveryResult.Message.Value)
                       .WithMessageBody(
                            GetMessageBody(
                                deliveryResult.Message.Value,
                                options.ValueSerializer,
                                () => new(MessageComponentType.Value, deliveryResult.Topic, deliveryResult.Message.Headers)));
            }

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);
        }

        public override void Enrich<TKey, TValue>(Activity activity, DeliveryReport<TKey, TValue> deliveryReport, IKafkaProducerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (deliveryReport is null)
            {
                throw new ArgumentNullException(nameof(deliveryReport), $"{nameof(deliveryReport)} cannot be null.");
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithError(deliveryReport.Error)
                .WithTopic(deliveryReport.Topic)
                .WithOffset(deliveryReport.Offset)
                .WithPartition(deliveryReport.Partition)
                .WithOperation(OperationNames.PublishOperation)
                .WithBootstrapServers(options.ProducerConfig!.BootstrapServers);

            if (deliveryReport.Message is not null)
            {
                builder.WithMessageId(options.MessageIdHandler?.Invoke(deliveryReport.Message.Value))
                       .WithMessageKey(deliveryReport.Message.Key)
                       .WithMessageValue(deliveryReport.Message.Value)
                       .WithMessageBody(
                            GetMessageBody(
                                deliveryReport.Message.Value,
                                options.ValueSerializer,
                                () => new(MessageComponentType.Value, deliveryReport.Topic, deliveryReport.Message.Headers)));
            }

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);
        }

        public override void Enrich<TKey, TValue>(Activity activity, DeliveryResult<TKey, TValue> deliveryResult, IKafkaProducerOptions<TKey, TValue> options)
        {
            if (activity is null)
            {
                return;
            }

            if (deliveryResult is null)
            {
                throw new ArgumentNullException(nameof(deliveryResult), $"{nameof(deliveryResult)} cannot be null.");
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            using var builder = new KafkaActivityAttributesBuilder()
                .WithTopic(deliveryResult.Topic)
                .WithOffset(deliveryResult.Offset)
                .WithPartition(deliveryResult.Partition)
                .WithOperation(OperationNames.PublishOperation)
                .WithBootstrapServers(options.ProducerConfig!.BootstrapServers);

            if (deliveryResult.Message is not null)
            {
                builder.WithMessageId(options.MessageIdHandler?.Invoke(deliveryResult.Message.Value))
                       .WithMessageKey(deliveryResult.Message.Key)
                       .WithMessageValue(deliveryResult.Message.Value)
                       .WithMessageBody(
                            GetMessageBody(
                                deliveryResult.Message.Value,
                                options.ValueSerializer,
                                () => new(MessageComponentType.Value, deliveryResult.Topic, deliveryResult.Message.Headers)));
            }

            var attributes = builder.Build();

            EnrichInternal(activity, attributes);
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
