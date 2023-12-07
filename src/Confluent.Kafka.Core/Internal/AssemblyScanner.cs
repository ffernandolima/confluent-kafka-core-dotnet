using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Internal
{
    internal static class AssemblyScanner
    {
        public static Type[] Scan(IEnumerable<Assembly> scanningAssemblies, Func<Type, bool> scanningFilter)
        {
            if (scanningAssemblies is null)
            {
                throw new ArgumentNullException(nameof(scanningAssemblies), $"{nameof(scanningAssemblies)} cannot be null.");
            }

            if (scanningFilter is null)
            {
                throw new ArgumentNullException(nameof(scanningFilter), $"{nameof(scanningFilter)} cannot be null.");
            }

            var assemblyTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(scannedAssembly => !scanningAssemblies.Any() || scanningAssemblies.Contains(scannedAssembly))
                .SelectMany(scannedAssembly => scannedAssembly.GetTypes())
                .Where(scanningFilter)
                .ToArray();

            return assemblyTypes;
        }
    }
}
