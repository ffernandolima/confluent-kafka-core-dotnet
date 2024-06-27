using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class NoopDiagnosticsManager : DiagnosticsManagerBase
    {
        private static readonly Lazy<NoopDiagnosticsManager> Factory = new(
            () => new NoopDiagnosticsManager(), isThreadSafe: true);

        public static NoopDiagnosticsManager Instance => Factory.Value;

        private NoopDiagnosticsManager()
        { }

        protected override ActivitySourceBase ActivitySource { get; } = null;
        protected override ActivityEnricherBase ActivityEnricher { get; } = null;
        protected override DistributedContextPropagator Propagator { get; } = null;
    }
}
