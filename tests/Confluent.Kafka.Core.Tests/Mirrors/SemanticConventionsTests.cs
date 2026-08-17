using Confluent.Kafka.Core.Diagnostics.Internal;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Mirrors
{
    /// <summary>
    /// The enrichment tests reference these constants symbolically, so changing a constant's value
    /// keeps them green while the emitted attribute silently changes. 
    /// These pin the literal strings against OpenTelemetry semantic conventions v1.44.0.
    /// </summary>
    public sealed class SemanticConventionsTests
    {
        [Theory]
        [InlineData("messaging.client.id", SemanticConventions.Messaging.ClientId)]
        [InlineData("messaging.destination.name", SemanticConventions.Messaging.DestinationName)]
        [InlineData("messaging.message.body.size", SemanticConventions.Messaging.MessageBodySize)]
        [InlineData("messaging.message.id", SemanticConventions.Messaging.MessageId)]
        [InlineData("messaging.operation.name", SemanticConventions.Messaging.OperationName)]
        [InlineData("messaging.operation.type", SemanticConventions.Messaging.OperationType)]
        [InlineData("messaging.system", SemanticConventions.Messaging.System)]
        [InlineData("error.type", SemanticConventions.Messaging.ErrorType)]
        [InlineData("server.address", SemanticConventions.Messaging.ServerAddress)]
        [InlineData("server.port", SemanticConventions.Messaging.ServerPort)]
        [InlineData("messaging.consumer.group.name", SemanticConventions.Messaging.Kafka.ConsumerGroupName)]
        [InlineData("messaging.destination.partition.id", SemanticConventions.Messaging.Kafka.DestinationPartitionId)]
        [InlineData("messaging.kafka.message.key", SemanticConventions.Messaging.Kafka.MessageKey)]
        [InlineData("messaging.kafka.message.tombstone", SemanticConventions.Messaging.Kafka.MessageTombstone)]
        [InlineData("messaging.kafka.offset", SemanticConventions.Messaging.Kafka.MessageOffset)]
        public void Attribute_MatchesTheConvention(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Not OpenTelemetry attributes; pinned so they are not renamed by accident either.
        /// </summary>
        [Theory]
        [InlineData("messaging.kafka.result.is_error", SemanticConventions.Messaging.Kafka.ResultIsError)]
        [InlineData("messaging.kafka.result.error_code", SemanticConventions.Messaging.Kafka.ResultErrorCode)]
        [InlineData("messaging.kafka.result.error_reason", SemanticConventions.Messaging.Kafka.ResultErrorReason)]
        [InlineData("messaging.kafka.processing.is_error", SemanticConventions.Messaging.Kafka.ProcessingIsError)]
        public void CustomAttribute_IsUnchanged(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// The operation name is free-form.
        /// Publishing maps to "send", which is the one place the two differ.
        /// </summary>
        [Theory]
        [InlineData("publish", OperationNames.PublishOperation)]
        [InlineData("receive", OperationNames.ReceiveOperation)]
        [InlineData("process", OperationNames.ProcessOperation)]
        public void OperationName_MatchesTheConvention(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// messaging.operation.type is a closed set.
        /// </summary>
        [Theory]
        [InlineData("send", OperationTypes.Send)]
        [InlineData("receive", OperationTypes.Receive)]
        [InlineData("process", OperationTypes.Process)]
        public void OperationType_MatchesTheConvention(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("messaging.client.operation.duration", SemanticConventions.Metrics.ClientOperationDuration)]
        [InlineData("messaging.client.sent.messages", SemanticConventions.Metrics.ClientSentMessages)]
        [InlineData("messaging.client.consumed.messages", SemanticConventions.Metrics.ClientConsumedMessages)]
        [InlineData("messaging.process.duration", SemanticConventions.Metrics.ProcessDuration)]
        public void Metric_MatchesTheConvention(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("s", SemanticConventions.Metrics.DurationUnit)]
        [InlineData("{message}", SemanticConventions.Metrics.MessageUnit)]
        public void MetricUnit_MatchesTheConvention(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// The ExplicitBucketBoundaries advisory parameter both duration histograms are specified with.
        /// </summary>
        [Fact]
        public void DurationBuckets_MatchTheConvention()
        {
            Assert.Equal<double>(
                [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10],
                SemanticConventions.Metrics.DurationBuckets);
        }
    }
}
