namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaDiagnosticsManager : KafkaDiagnosticsManagerBase
    {
        protected override ActivitySourceBase ActivitySource { get; }
        protected override IKafkaActivityEnricher ActivityEnricher { get; }

        public KafkaDiagnosticsManager(KafkaEnrichmentOptions options)
        {
            ActivitySource = new KafkaActivitySource();
            ActivityEnricher = new KafkaActivityEnricher(options);
        }
    }
}
