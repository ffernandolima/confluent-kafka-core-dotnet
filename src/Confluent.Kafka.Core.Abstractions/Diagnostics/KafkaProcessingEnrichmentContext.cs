using Confluent.Kafka.Core.Hosting;

namespace Confluent.Kafka.Core.Diagnostics
{
    public class KafkaProcessingEnrichmentContext : KafkaConsumptionEnrichmentContext
    {
        public IKafkaConsumerWorkerConfig WorkerConfig { get; init; }
    }
}
