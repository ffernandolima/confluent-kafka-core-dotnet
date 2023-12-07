using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaDiagnosticsManager : DiagnosticsManagerBase
    {
        private static readonly Lazy<KafkaDiagnosticsManager> Factory = new(
            () => new KafkaDiagnosticsManager(), isThreadSafe: true);

        public static KafkaDiagnosticsManager Instance => Factory.Value;

        protected override ActivitySourceBase ActivitySource { get; } = new KafkaActivitySource();
        protected override ActivityEnricherBase ActivityEnricher { get; } = new KafkaActivityEnricher();
    }
}
