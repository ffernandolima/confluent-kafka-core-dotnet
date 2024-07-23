using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaEnrichmentOptions
    {
        public Action<Activity, KafkaConsumptionFailureEnrichmentContext> EnrichConsumptionFailure { get; set; }
        public Action<Activity, KafkaConsumptionEnrichmentContext> EnrichConsumption { get; set; }
        public Action<Activity, KafkaProductionFailureEnrichmentContext> EnrichProductionFailure { get; set; }
        public Action<Activity, KafkaSyncProductionEnrichmentContext> EnrichSyncProduction { get; set; }
        public Action<Activity, KafkaAsyncProductionEnrichmentContext> EnrichAsyncProduction { get; set; }
        public Action<Activity, KafkaProcessingFailureEnrichmentContext> EnrichProcessingFailure { get; set; }
        public Action<Activity, KafkaProcessingEnrichmentContext> EnrichProcessing { get; set; }
    }
}
