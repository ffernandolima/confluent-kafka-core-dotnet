using Confluent.Kafka.Core.Consumer;

namespace Confluent.Kafka.Core.Diagnostics
{
    public class KafkaConsumptionEnrichmentContext
    {
        public string Topic { get; init; }
        public Partition Partition { get; init; }
        public Offset Offset { get; init; }
        public object Key { get; init; }
        public object Value { get; init; }
        public Timestamp Timestamp { get; init; }
        public Headers Headers { get; init; }
        public IKafkaConsumerConfig ConsumerConfig { get; init; }
    }
}
