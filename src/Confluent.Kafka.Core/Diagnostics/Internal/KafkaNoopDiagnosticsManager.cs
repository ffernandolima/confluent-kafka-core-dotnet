using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaNoopDiagnosticsManager : KafkaDiagnosticsManagerBase
    {
        private static readonly Lazy<KafkaNoopDiagnosticsManager> Factory = new(
            () => new KafkaNoopDiagnosticsManager(), isThreadSafe: true);

        public static KafkaNoopDiagnosticsManager Instance => Factory.Value;

        protected override ActivitySourceBase ActivitySource { get; } = null;
        protected override IKafkaActivityEnricher ActivityEnricher { get; } = null;
        protected override DistributedContextPropagator Propagator { get; } = null;

        private KafkaNoopDiagnosticsManager()
        { }
    }
}
