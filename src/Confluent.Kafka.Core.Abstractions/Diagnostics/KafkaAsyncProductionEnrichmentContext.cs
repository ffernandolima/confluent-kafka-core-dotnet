using Confluent.Kafka.Core.Producer;

namespace Confluent.Kafka.Core.Diagnostics
{
    public class KafkaAsyncProductionEnrichmentContext
    {
        public string Topic { get; init; }
        public Partition Partition { get; init; }
        public Offset Offset { get; init; }
        public object Key { get; init; }
        public object Value { get; init; }
        public Timestamp Timestamp { get; init; }
        public Headers Headers { get; init; }
        public PersistenceStatus Status { get; init; }
        public IKafkaProducerConfig ProducerConfig { get; init; }
    }
}
