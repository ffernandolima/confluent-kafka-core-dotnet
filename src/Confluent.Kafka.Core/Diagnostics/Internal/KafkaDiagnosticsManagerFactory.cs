using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaDiagnosticsManagerFactory : IKafkaDiagnosticsManagerFactory
    {
        private static readonly Lazy<KafkaDiagnosticsManagerFactory> Factory = new(
            () => new KafkaDiagnosticsManagerFactory(), isThreadSafe: true);

        public static KafkaDiagnosticsManagerFactory Instance => Factory.Value;

        private KafkaDiagnosticsManagerFactory()
        { }

        public IKafkaDiagnosticsManager GetOrCreateDiagnosticsManager(
            IServiceProvider serviceProvider,
            bool enableDiagnostics,
            Action<IKafkaEnrichmentOptionsBuilder> configureOptions)
        {
            var diagnosticsManager = !enableDiagnostics
                ? KafkaNoopDiagnosticsManager.Instance
                : GetOrCreateDiagnosticsManager(serviceProvider, configureOptions);

            return diagnosticsManager;
        }

        public IKafkaDiagnosticsManager GetOrCreateDiagnosticsManager(
            IServiceProvider serviceProvider,
            Action<IKafkaEnrichmentOptionsBuilder> configureOptions)
        {
            var diagnosticsManager = serviceProvider?.GetService<IKafkaDiagnosticsManager>() ??
                CreateDiagnosticsManager(
                    serviceProvider,
                    (_, builder) => configureOptions?.Invoke(builder));

            return diagnosticsManager;
        }

        public IKafkaDiagnosticsManager CreateDiagnosticsManager(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IKafkaEnrichmentOptionsBuilder> configureOptions)
        {
            var options = KafkaEnrichmentOptionsBuilder.Build(serviceProvider, configureOptions);

            var diagnosticsManager = new KafkaDiagnosticsManager(options);

            return diagnosticsManager;
        }
    }
}
