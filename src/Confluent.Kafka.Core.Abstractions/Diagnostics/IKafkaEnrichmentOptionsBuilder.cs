using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics
{
    public interface IKafkaEnrichmentOptionsBuilder
    {
        IKafkaEnrichmentOptionsBuilder WithConsumptionEnrichment(Action<Activity, KafkaConsumptionEnrichmentContext> enrichConsumption);

        IKafkaEnrichmentOptionsBuilder WithConsumptionFailureEnrichment(Action<Activity, KafkaConsumptionFailureEnrichmentContext> enrichConsumptionFailure);

        IKafkaEnrichmentOptionsBuilder WithSyncProductionEnrichment(Action<Activity, KafkaSyncProductionEnrichmentContext> enrichSyncProduction);

        IKafkaEnrichmentOptionsBuilder WithAsyncProductionEnrichment(Action<Activity, KafkaAsyncProductionEnrichmentContext> enrichAsyncProduction);

        IKafkaEnrichmentOptionsBuilder WithProductionFailureEnrichment(Action<Activity, KafkaProductionFailureEnrichmentContext> enrichProductionFailure);

        IKafkaEnrichmentOptionsBuilder WithProcessingEnrichment(Action<Activity, KafkaProcessingEnrichmentContext> enrichProcessing);

        IKafkaEnrichmentOptionsBuilder WithProcessingFailureEnrichment(Action<Activity, KafkaProcessingFailureEnrichmentContext> enrichProcessingFailure);
    }
}
