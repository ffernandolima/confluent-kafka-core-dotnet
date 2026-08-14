using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Mirrors
{
    internal static class MirrorAssert
    {
        /// <summary>
        /// Asserts every settable public property declared on <paramref name="sourceType"/> has a
        /// matching <c>With*</c> on <paramref name="builderInterface"/>, and a matching property on
        /// <paramref name="mirrorInterface"/> when one is supplied.
        /// </summary>
        public static void IsComplete(Type sourceType, Type builderInterface, Type mirrorInterface = null)
        {
            var properties = GetMirrorableProperties(sourceType);

            // Guards against the check silently becoming vacuous if the properties move to a base type.
            Assert.True(
                properties.Count > 0,
                $"No mirrorable properties were discovered on {sourceType.FullName}. " +
                "The declaring type has probably changed and this test needs to be widened.");

            var missing = new List<string>();

            foreach (var property in properties)
            {
                if (mirrorInterface is not null && !HasProperty(mirrorInterface, property.Name))
                {
                    missing.Add($"{mirrorInterface.Name}.{property.Name}");
                }

                if (!HasMethod(builderInterface, $"With{property.Name}"))
                {
                    missing.Add($"{builderInterface.Name}.With{property.Name}");
                }
            }

            Assert.True(
                missing.Count == 0,
                $"{sourceType.FullName} exposes members that Confluent.Kafka.Core does not mirror, " +
                "usually because the source type gained a property without the mirror being synced. " +
                $"Missing: {string.Join(", ", missing)}");
        }

        private static IReadOnlyList<PropertyInfo> GetMirrorableProperties(Type sourceType)
        {
            return [.. sourceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(property => property.CanRead && property.CanWrite)
                .Where(property => property.GetIndexParameters().Length == 0)
                .Where(property => property.GetCustomAttribute<ObsoleteAttribute>() is null)
                .Where(property => !IsExperimental(property))
                .OrderBy(property => property.Name, StringComparer.Ordinal)];
        }

        // Vendors mark evaluation-only APIs [Experimental]; mirroring one would republish an unstable
        // API to consumers without the diagnostic that warns them about it.
        private static bool IsExperimental(PropertyInfo property)
        {
            return property
                .GetCustomAttributes()
                .Any(attribute => attribute.GetType().Name == "ExperimentalAttribute");
        }

        private static bool HasProperty(Type interfaceType, string name)
        {
            return WithInheritedInterfaces(interfaceType)
                .Any(type => type.GetProperty(name) is not null);
        }

        private static bool HasMethod(Type interfaceType, string name)
        {
            return WithInheritedInterfaces(interfaceType)
                .SelectMany(type => type.GetMethods())
                .Any(method => string.Equals(method.Name, name, StringComparison.Ordinal));
        }

        // Interface reflection does not surface inherited members, so base interfaces are walked explicitly.
        private static IEnumerable<Type> WithInheritedInterfaces(Type interfaceType)
        {
            return new[] { interfaceType }.Concat(interfaceType.GetInterfaces());
        }
    }
}
