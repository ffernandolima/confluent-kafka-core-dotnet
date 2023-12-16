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
                    attribute.ServerAddress = serversInfo.ServerHostnames ?? serversInfo.ServerIpAddresses;
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
            AppendAction(attribute => attribute.DestinationPartition = partition);
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
                if (messageKey is not null && messageKey is not Null && messageKey is not Ignore)
                {
                    attribute.MessageKey = messageKey.ToString();
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

        public KafkaActivityAttributesBuilder WithOperation(string operation)
        {
            AppendAction(attribute => attribute.Operation = operation);
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
                    attribute.ExceptionType = exception.GetType().ExtractTypeName();
                    attribute.ExceptionMessage = exception.Message;
                    attribute.ExceptionStackTrace = exception.ToString();
                }
            });
            return this;
        }
    }
}
