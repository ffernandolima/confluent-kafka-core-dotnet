namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    /// <summary>
    /// Semantic Conventions v1.44.0
    /// </summary>
    internal static class SemanticConventions
    {
        /// <summary>
        /// https://github.com/open-telemetry/semantic-conventions/blob/v1.44.0/docs/messaging/messaging-spans.md
        /// </summary>
        internal static class Messaging
        {
            public const string ClientId = "messaging.client.id";
            public const string DestinationName = "messaging.destination.name";
            public const string MessageBodySize = "messaging.message.body.size";
            public const string MessageId = "messaging.message.id";
            public const string OperationName = "messaging.operation.name";
            public const string OperationType = "messaging.operation.type";
            public const string System = "messaging.system";
            public const string ErrorType = "error.type";
            public const string ServerAddress = "server.address";
            public const string ServerPort = "server.port";

            /// <summary>
            /// https://github.com/open-telemetry/semantic-conventions/blob/v1.44.0/docs/messaging/kafka.md#span-attributes
            /// </summary>
            internal static class Kafka
            {
                public const string ConsumerGroupName = "messaging.consumer.group.name";
                public const string DestinationPartitionId = "messaging.destination.partition.id";
                public const string MessageKey = "messaging.kafka.message.key";
                public const string MessageTombstone = "messaging.kafka.message.tombstone";
                public const string MessageOffset = "messaging.kafka.offset";

                /// <summary>
                /// Not part of the OpenTelemetry conventions. These carry Kafka error detail that
                /// <see cref="ErrorType"/> cannot express, so they are retained alongside it.
                /// </summary>
                public const string ResultIsError = "messaging.kafka.result.is_error";
                public const string ResultErrorCode = "messaging.kafka.result.error_code";
                public const string ResultErrorReason = "messaging.kafka.result.error_reason";
                public const string ProcessingIsError = "messaging.kafka.processing.is_error";
            }
        }

        /// <summary>
        /// https://github.com/open-telemetry/semantic-conventions/blob/v1.44.0/docs/messaging/messaging-metrics.md
        /// </summary>
        internal static class Metrics
        {
            public const string ClientOperationDuration = "messaging.client.operation.duration";
            public const string ClientSentMessages = "messaging.client.sent.messages";
            public const string ClientConsumedMessages = "messaging.client.consumed.messages";
            public const string ProcessDuration = "messaging.process.duration";

            public const string DurationUnit = "s";
            public const string MessageUnit = "{message}";

            /// <summary>
            /// The ExplicitBucketBoundaries advisory parameter both duration histograms are specified with.
            /// </summary>
            public static readonly double[] DurationBuckets = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];
        }
    }
}
