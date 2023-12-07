namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaActivityAttributes
    {
        public string System { get; set; } = "kafka";
        public string ClientId { get; set; } = "rdkafka";
        public string Operation { get; set; }
        public string MessageKey { get; set; }
        public object MessageId { get; set; }
        public int? MessageBodySize { get; set; }
        public Offset MessageOffset { get; set; }
        public bool MessageTombstone { get; set; }
        public string ConsumerGroup { get; set; }
        public string DestinationName { get; set; }
        public Partition? DestinationPartition { get; set; }
        public bool ResultIsError { get; set; }
        public ErrorCode? ResultErrorCode { get; set; }
        public string ResultErrorReason { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ServerAddress { get; set; }
        public string NetworkTransport { get; set; } = "tcp";
    }
}
