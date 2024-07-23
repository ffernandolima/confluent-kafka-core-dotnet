using System;

namespace Confluent.Kafka.Core.Diagnostics
{
    public class KafkaConsumptionFailureEnrichmentContext : KafkaConsumptionEnrichmentContext
    {
        public Error Error { get; init; }
        public Exception Exception { get; init; }
    }
}
