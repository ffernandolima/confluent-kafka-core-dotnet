namespace Confluent.Kafka.Core.Diagnostics
{
    public class KafkaSyncProductionEnrichmentContext : KafkaAsyncProductionEnrichmentContext
    {
        public Error Error { get; init; }
    }
}
