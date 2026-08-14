using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models.Internal;
using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    /// <summary>
    /// Records the messaging metrics. Deliberately independent of <see cref="Activity"/>: 
    /// a span is absent whenever no listener is registered and whenever sampling drops it, 
    /// so deriving metrics from one would undercount in proportion to the sampling rate.
    /// </summary>
    internal static class KafkaMetricsRecorder
    {
        public static void RecordProduction(
            long startTimestamp,
            string bootstrapServers,
            string topic,
            Partition partition,
            Error error = null,
            Exception exception = null)
        {
            var meter = KafkaMeter.Instance;

            if (!meter.ClientOperationDuration.Enabled && !meter.ClientSentMessages.Enabled)
            {
                return;
            }

            var measurementValue = GetElapsedSeconds(startTimestamp);

            var tags = BuildTags(
                OperationNames.PublishOperation,
                OperationTypes.Send,
                bootstrapServers,
                groupId: null,
                topic,
                partition,
                GetErrorType(error, exception));

            meter.ClientOperationDuration.Record(measurementValue, in tags);

            // The counter measures attempts, so a failed send is counted and told apart by error.type.
            meter.ClientSentMessages.Add(1, in tags);
        }

        public static void RecordConsumption(
            long startTimestamp,
            string bootstrapServers,
            string groupId,
            string topic,
            Partition partition,
            Error error = null,
            Exception exception = null)
        {
            var meter = KafkaMeter.Instance;

            if (!meter.ClientOperationDuration.Enabled && !meter.ClientConsumedMessages.Enabled)
            {
                return;
            }

            var measurementValue = GetElapsedSeconds(startTimestamp);

            var tags = BuildTags(
                OperationNames.ReceiveOperation,
                OperationTypes.Receive,
                bootstrapServers,
                groupId,
                topic,
                partition,
                GetErrorType(error, exception));

            meter.ClientOperationDuration.Record(measurementValue, in tags);
            meter.ClientConsumedMessages.Add(1, in tags);
        }

        public static void RecordProcessing(
            long startTimestamp,
            string bootstrapServers,
            string groupId,
            string topic,
            Partition partition,
            Exception exception = null)
        {
            var meter = KafkaMeter.Instance;

            if (!meter.ProcessDuration.Enabled)
            {
                return;
            }

            var measurementValue = GetElapsedSeconds(startTimestamp);

            var tags = BuildTags(
                OperationNames.ProcessOperation,
                OperationTypes.Process,
                bootstrapServers,
                groupId,
                topic,
                partition,
                GetErrorType(error: null, exception));

            meter.ProcessDuration.Record(measurementValue, in tags);
        }

        /// <summary>
        /// Tag order mirrors the span attribute order in the enricher, so the two are read side by side.
        /// </summary>
        private static TagList BuildTags(
            string operationName,
            string operationType,
            string bootstrapServers,
            string groupId,
            string topic,
            Partition partition,
            string errorType)
        {
            var tags = new TagList
            {
                { SemanticConventions.Messaging.System, KafkaSenderConstants.KafkaSystem },
                { SemanticConventions.Messaging.OperationName, operationName },
                { SemanticConventions.Messaging.OperationType, operationType }
            };

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                tags.Add(SemanticConventions.Messaging.Kafka.ConsumerGroupName, groupId);
            }

            if (!string.IsNullOrWhiteSpace(topic))
            {
                tags.Add(SemanticConventions.Messaging.DestinationName, topic);
            }

            // Partition.Any and the other special values describe an intent rather than a partition, so they are not reported as one.
            if (partition.Value >= 0)
            {
                tags.Add(SemanticConventions.Messaging.Kafka.DestinationPartitionId, partition.Value.ToString());
            }

            if (!string.IsNullOrWhiteSpace(errorType))
            {
                tags.Add(SemanticConventions.Messaging.ErrorType, errorType);
            }

            var serversInfo = KafkaServersInfo.Parse(bootstrapServers);

            if (serversInfo is not null)
            {
                if (!string.IsNullOrWhiteSpace(serversInfo.ServerAddress))
                {
                    tags.Add(SemanticConventions.Messaging.ServerAddress, serversInfo.ServerAddress);
                }

                if (serversInfo.ServerPort.HasValue)
                {
                    tags.Add(SemanticConventions.Messaging.ServerPort, serversInfo.ServerPort.Value);
                }
            }

            return tags;
        }

        private static string GetErrorType(Error error, Exception exception)
        {
            if (error is not null && error.IsError)
            {
                return error.Code.ToString();
            }

            return exception?.GetType().ExtractTypeName();
        }

        /// <summary>
        /// Stopwatch.GetElapsedTime is unavailable on netstandard2.0, and the metric unit is seconds anyway.
        /// </summary>
        private static double GetElapsedSeconds(long startTimestamp)
            => (Stopwatch.GetTimestamp() - startTimestamp) / (double)Stopwatch.Frequency;
    }
}
