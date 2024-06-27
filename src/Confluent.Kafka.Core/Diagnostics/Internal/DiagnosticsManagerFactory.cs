using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class DiagnosticsManagerFactory
    {
        private static readonly Lazy<DiagnosticsManagerFactory> Factory = new(
            () => new DiagnosticsManagerFactory(), isThreadSafe: true);

        public static DiagnosticsManagerFactory Instance => Factory.Value;

        private DiagnosticsManagerFactory()
        { }

        public IDiagnosticsManager GetDiagnosticsManager(IServiceProvider serviceProvider, bool enableDiagnostics)
        {
            var diagnosticsManager = !enableDiagnostics
                ? serviceProvider?.GetKeyedService<IDiagnosticsManager>(nameof(NoopDiagnosticsManager)) ?? NoopDiagnosticsManager.Instance
                : serviceProvider?.GetKeyedService<IDiagnosticsManager>(nameof(KafkaDiagnosticsManager)) ?? KafkaDiagnosticsManager.Instance;

            return diagnosticsManager;
        }
    }
}
