using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaActivityAttributes
    {
        public string System { get; set; } = "kafka";
        public string ClientId { get; set; } = "rdkafka";
        public string OperationName { get; set; }
        public string OperationType { get; set; }
        public string MessageKey { get; set; }
        public object MessageId { get; set; }
        public int? MessageBodySize { get; set; }
        public Offset? MessageOffset { get; set; }
        public bool MessageTombstone { get; set; }
        public string ConsumerGroup { get; set; }
        public string DestinationName { get; set; }
        public string DestinationPartitionId { get; set; }
        public bool ResultIsError { get; set; }
        public ErrorCode? ResultErrorCode { get; set; }
        public string ResultErrorReason { get; set; }
        public string ErrorType { get; set; }
        public Exception Exception { get; set; }
        public string ServerAddress { get; set; }
        public int? ServerPort { get; set; }
    }
}
