using Confluent.Kafka.Core.Internal;
using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaEnrichmentOptionsBuilder :
        FunctionalBuilder<KafkaEnrichmentOptions, KafkaEnrichmentOptionsBuilder>,
        IKafkaEnrichmentOptionsBuilder
    {
        public IKafkaEnrichmentOptionsBuilder WithConsumptionFailureEnrichment(
            Action<Activity, KafkaConsumptionFailureEnrichmentContext> enrichConsumptionFailure)
        {
            AppendAction(options => options.EnrichConsumptionFailure = enrichConsumptionFailure);
            return this;
        }

        public IKafkaEnrichmentOptionsBuilder WithConsumptionEnrichment(
            Action<Activity, KafkaConsumptionEnrichmentContext> enrichConsumption)
        {
            AppendAction(options => options.EnrichConsumption = enrichConsumption);
            return this;
        }

        public IKafkaEnrichmentOptionsBuilder WithProductionFailureEnrichment(
            Action<Activity, KafkaProductionFailureEnrichmentContext> enrichProductionFailure)
        {
            AppendAction(options => options.EnrichProductionFailure = enrichProductionFailure);
            return this;
        }

        public IKafkaEnrichmentOptionsBuilder WithSyncProductionEnrichment(
            Action<Activity, KafkaSyncProductionEnrichmentContext> enrichSyncProduction)
        {
            AppendAction(options => options.EnrichSyncProduction = enrichSyncProduction);
            return this;
        }

        public IKafkaEnrichmentOptionsBuilder WithAsyncProductionEnrichment(
            Action<Activity, KafkaAsyncProductionEnrichmentContext> enrichAsyncProduction)
        {
            AppendAction(options => options.EnrichAsyncProduction = enrichAsyncProduction);
            return this;
        }

        public IKafkaEnrichmentOptionsBuilder WithProcessingFailureEnrichment(
            Action<Activity, KafkaProcessingFailureEnrichmentContext> enrichProcessingFailure)
        {
            AppendAction(options => options.EnrichProcessingFailure = enrichProcessingFailure);
            return this;
        }

        public IKafkaEnrichmentOptionsBuilder WithProcessingEnrichment(
            Action<Activity, KafkaProcessingEnrichmentContext> enrichProcessing)
        {
            AppendAction(options => options.EnrichProcessing = enrichProcessing);
            return this;
        }

        public static KafkaEnrichmentOptions Build(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IKafkaEnrichmentOptionsBuilder> configureOptions)
        {
            using var builder = new KafkaEnrichmentOptionsBuilder();

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}
