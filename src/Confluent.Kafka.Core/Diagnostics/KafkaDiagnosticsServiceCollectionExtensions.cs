using System;
using System.Collections.Generic;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaDiagnosticsServiceCollectionExtensions
    {
        private static readonly Dictionary<string, Func<IServiceProvider, object, IDiagnosticsManager>> _mappings = new()
        {
            { nameof(KafkaDiagnosticsManager), (_, _) => KafkaDiagnosticsManager.Instance },
            { nameof(NoopDiagnosticsManager),  (_, _) => NoopDiagnosticsManager.Instance  }
        };

        public static IServiceCollection AddKafkaDiagnostics(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            foreach (var mapping in _mappings)
            {
                services.TryAddKeyedSingleton(mapping.Key, mapping.Value);
            }

            return services;
        }
    }
}
