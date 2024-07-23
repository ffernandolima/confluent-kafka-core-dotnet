using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaDiagnosticsServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaDiagnostics(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaEnrichmentOptionsBuilder> configureOptions = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton(serviceProvider =>
            {
                var diagnosticsManager = KafkaDiagnosticsManagerFactory.Instance.CreateDiagnosticsManager(
                    serviceProvider,
                    configureOptions);

                return diagnosticsManager;
            });

            return services;
        }
    }
}
