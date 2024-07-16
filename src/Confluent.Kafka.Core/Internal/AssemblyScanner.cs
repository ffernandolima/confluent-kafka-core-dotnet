using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Internal
{
    internal static class AssemblyScanner
    {
        public static Type[] Scan(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        {
            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var assemblyTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(loadedAssembly => !assemblies.Any(assembly => assembly is not null) || assemblies.Contains(loadedAssembly))
                .SelectMany(loadedAssembly => loadedAssembly.GetTypes())
                .Where(predicate)
                .ToArray();

            return assemblyTypes;
        }
    }
}
