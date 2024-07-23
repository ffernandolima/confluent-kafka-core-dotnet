using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal interface IKafkaDiagnosticsManagerFactory
    {
        IKafkaDiagnosticsManager GetOrCreateDiagnosticsManager(
            IServiceProvider serviceProvider,
            bool enableDiagnostics,
            Action<IKafkaEnrichmentOptionsBuilder> configureOptions);

        IKafkaDiagnosticsManager GetOrCreateDiagnosticsManager(
            IServiceProvider serviceProvider,
            Action<IKafkaEnrichmentOptionsBuilder> configureOptions);

        IKafkaDiagnosticsManager CreateDiagnosticsManager(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IKafkaEnrichmentOptionsBuilder> configureOptions);
    }
}
