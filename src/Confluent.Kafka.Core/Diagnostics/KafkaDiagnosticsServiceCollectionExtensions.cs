using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaDiagnosticsServiceCollectionExtensions
    {
        private static readonly IDictionary<string, Func<IServiceProvider, object, IDiagnosticsManager>> _mappings =
            new Dictionary<string, Func<IServiceProvider, object, IDiagnosticsManager>>()
        {
            { DiagnosticsManagerConstants.KafkaDiagnosticsManager, (_, _) => KafkaDiagnosticsManager.Instance },
            { DiagnosticsManagerConstants.NoopDiagnosticsManager,  (_, _) => NoopDiagnosticsManager.Instance  }
        }
#if NET8_0_OR_GREATER
        .ToFrozenDictionary();
#else
        ;
#endif

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
