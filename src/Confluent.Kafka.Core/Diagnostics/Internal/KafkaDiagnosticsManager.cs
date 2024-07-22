using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaDiagnosticsManager : DiagnosticsManagerBase
    {
        private static readonly Lazy<KafkaDiagnosticsManager> Factory = new(
            () => new KafkaDiagnosticsManager(), isThreadSafe: true);

        public static KafkaDiagnosticsManager Instance => Factory.Value;

        private KafkaDiagnosticsManager()
        { }

        protected override ActivitySourceBase ActivitySource { get; } = new KafkaActivitySource();
        protected override IKafkaActivityEnricher ActivityEnricher { get; } = new KafkaActivityEnricher();
    }
}
