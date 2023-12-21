using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal static class DiagnosticsManagerFactory
    {
        public static IDiagnosticsManager GetDiagnosticsManager(IServiceProvider serviceProvider, bool enableDiagnostics)
        {
            var diagnosticsManager = !enableDiagnostics
                ? serviceProvider?.GetKeyedService<IDiagnosticsManager>(nameof(NoopDiagnosticsManager)) ?? NoopDiagnosticsManager.Instance
                : serviceProvider?.GetKeyedService<IDiagnosticsManager>(nameof(KafkaDiagnosticsManager)) ?? KafkaDiagnosticsManager.Instance;

            return diagnosticsManager;
        }
    }
}
