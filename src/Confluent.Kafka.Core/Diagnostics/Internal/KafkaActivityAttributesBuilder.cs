using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models.Internal;
using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaActivityAttributesBuilder : FunctionalBuilder<KafkaActivityAttributes, KafkaActivityAttributesBuilder>
    {
        public KafkaActivityAttributesBuilder WithBootstrapServers(string bootstrapServers)
        {
            AppendAction(attribute =>
            {
                var serversInfo = KafkaServersInfo.Parse(bootstrapServers);
                if (serversInfo is not null)
                {
                    attribute.ServerAddress = serversInfo.ServerAddress;
                    attribute.ServerPort = serversInfo.ServerPort;
                }
            });
            return this;
        }

        public KafkaActivityAttributesBuilder WithClientId(string clientId)
        {
            AppendAction(attribute =>
            {
                if (!string.IsNullOrWhiteSpace(clientId))
                {
                    attribute.ClientId = clientId;
                }
            });
            return this;
        }

        public KafkaActivityAttributesBuilder WithGroupId(string groupId)
        {
            AppendAction(attribute => attribute.ConsumerGroup = groupId);
            return this;
        }

        public KafkaActivityAttributesBuilder WithTopic(string topic)
        {
            AppendAction(attribute => attribute.DestinationName = topic);
            return this;
        }

        public KafkaActivityAttributesBuilder WithPartition(Partition partition)
        {
            AppendAction(attribute => attribute.DestinationPartitionId = partition.Value.ToString());
            return this;
        }

        public KafkaActivityAttributesBuilder WithOffset(Offset offset)
        {
            AppendAction(attribute => attribute.MessageOffset = offset);
            return this;
        }

        public KafkaActivityAttributesBuilder WithMessageKey(object messageKey)
        {
            AppendAction(attribute =>
            {
                if (messageKey is not null and not Null and not Ignore)
                {
                    attribute.MessageKey = messageKey is byte[] messageKeyBytes
                        ? EncodingFactory.Instance.CreateDefault().GetString(messageKeyBytes)
                        : messageKey.ToString();
                }
            });
            return this;
        }

        public KafkaActivityAttributesBuilder WithMessageValue(object messageValue)
        {
            AppendAction(attribute =>
            {
                if (!string.IsNullOrWhiteSpace(attribute.MessageKey) && messageValue is null)
                {
                    attribute.MessageTombstone = true;
                }
            });
            return this;
        }

        public KafkaActivityAttributesBuilder WithMessageId(object messageId)
        {
            AppendAction(attribute => attribute.MessageId = messageId);
            return this;
        }

        public KafkaActivityAttributesBuilder WithMessageBody(byte[] messageBody)
        {
            AppendAction(attribute => attribute.MessageBodySize = messageBody?.Length);
            return this;
        }

        public KafkaActivityAttributesBuilder WithOperation(string operationName, string operationType)
        {
            AppendAction(attribute =>
            {
                attribute.OperationName = operationName;
                attribute.OperationType = operationType;
            });
            return this;
        }

        public KafkaActivityAttributesBuilder WithError(Error error)
        {
            AppendAction(attribute =>
            {
                if (error is not null)
                {
                    if (attribute.ResultIsError = error.IsError)
                    {
                        attribute.ResultErrorCode = error.Code;
                        attribute.ResultErrorReason = error.Reason;
                        attribute.ErrorType ??= error.Code.ToString();
                    }
                }
            });
            return this;
        }

        public KafkaActivityAttributesBuilder WithException(Exception exception)
        {
            AppendAction(attribute =>
            {
                if (exception is not null)
                {
                    attribute.Exception = exception;
                    attribute.ErrorType ??= exception.GetType().ExtractTypeName();
                }
            });
            return this;
        }
    }
}
