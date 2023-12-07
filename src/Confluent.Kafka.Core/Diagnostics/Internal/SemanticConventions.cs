namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    /// <summary>
    /// Semantic Conventions v1.23.0
    /// </summary>
    internal static class SemanticConventions
    {
        /// <summary>
        /// https://github.com/open-telemetry/semantic-conventions/blob/v1.23.1/docs/messaging/messaging-spans.md
        /// </summary>
        internal static class Messaging
        {
            public const string ClientId = "messaging.client_id";
            public const string DestinationName = "messaging.destination.name";
            public const string MessageBodySize = "messaging.message.body.size";
            public const string MessageId = "messaging.message.id";
            public const string Operation = "messaging.operation";
            public const string System = "messaging.system";
            public const string NetworkTransport = "network.transport";
            public const string ServerAddress = "server.address";
            public const string ExceptionType = "exception.type";
            public const string ExceptionMessage = "exception.message";
            public const string ExceptionStackTrace = "exception.stacktrace";

            /// <summary>
            /// https://github.com/open-telemetry/semantic-conventions/blob/v1.23.1/docs/messaging/kafka.md#span-attributes
            /// </summary>
            internal static class Kafka
            {
                public const string ConsumerGroup = "messaging.kafka.consumer.group";
                public const string DestinationPartition = "messaging.kafka.destination.partition";
                public const string MessageKey = "messaging.kafka.message.key";
                public const string MessageOffset = "messaging.kafka.message.offset";
                public const string MessageTombstone = "messaging.kafka.message.tombstone";
                public const string ResultIsError = "messaging.kafka.result.is_error";
                public const string ResultErrorCode = "messaging.kafka.result.error_code";
                public const string ResultErrorReason = "messaging.kafka.result.error_reason";
            }
        }
    }
}
