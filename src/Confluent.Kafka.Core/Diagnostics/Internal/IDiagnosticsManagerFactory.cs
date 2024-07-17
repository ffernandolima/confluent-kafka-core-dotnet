using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal interface IDiagnosticsManagerFactory
    {
        IDiagnosticsManager GetDiagnosticsManager(IServiceProvider serviceProvider, bool enableDiagnostics);
    }
}
