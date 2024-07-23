using System;

namespace Confluent.Kafka.Core.Diagnostics
{
    public class KafkaProcessingFailureEnrichmentContext : KafkaProcessingEnrichmentContext
    {
        public Exception Exception { get; init; }
    }
}
