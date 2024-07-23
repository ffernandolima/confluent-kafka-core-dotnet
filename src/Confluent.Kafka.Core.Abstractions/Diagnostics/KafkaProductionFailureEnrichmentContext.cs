using System;

namespace Confluent.Kafka.Core.Diagnostics
{
    public class KafkaProductionFailureEnrichmentContext : KafkaSyncProductionEnrichmentContext
    {
        public Exception Exception { get; init; }
    }
}
