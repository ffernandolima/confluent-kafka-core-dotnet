using Confluent.Kafka.Core.Serialization.JsonCore;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Serialization
{
    /// <summary>
    /// The serializer settings builders are hand-maintained mirrors and not enforced by the compiler,
    /// so bumping System.Text.Json or Newtonsoft.Json can leave a new upstream property unreachable
    /// through the fluent API. These tests catch that.
    /// </summary>
    public sealed class SerializerMirrorConformanceTests
    {
        [Fact]
        public void JsonSerializerOptions_IsFullyMirrored()
        {
            AssertMirrored(typeof(JsonSerializerOptions), typeof(IJsonSerializerOptionsBuilder));
        }

        [Fact]
        public void JsonSerializerSettings_IsFullyMirrored()
        {
            AssertMirrored(typeof(JsonSerializerSettings), typeof(IJsonSerializerSettingsBuilder));
        }

        private static void AssertMirrored(Type settingsType, Type builderInterface)
        {
            var properties = GetMirrorableProperties(settingsType);

            Assert.True(
                properties.Count > 0,
                $"No mirrorable properties were discovered on {settingsType.FullName}. " +
                "The declaring type has probably changed upstream and this test needs to be widened.");

            var missing = properties
                .Where(property => !HasMethod(builderInterface, $"With{property.Name}"))
                .Select(property => $"{builderInterface.Name}.With{property.Name}")
                .ToList();

            Assert.True(
                missing.Count == 0,
                $"{settingsType.FullName} exposes members that Confluent.Kafka.Core does not mirror, " +
                "which usually means the package was upgraded without syncing the builder. " +
                $"Missing: {string.Join(", ", missing)}");
        }

        // Read-only collection properties such as JsonSerializerOptions.Converters and
        // TypeInfoResolverChain are excluded here; the builders expose them as mutating With* methods.
        private static IReadOnlyList<PropertyInfo> GetMirrorableProperties(Type settingsType)
        {
            return [.. settingsType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(property => property.CanRead && property.CanWrite)
                .Where(property => property.GetIndexParameters().Length == 0)
                .Where(property => property.GetCustomAttribute<ObsoleteAttribute>() is null)
                .OrderBy(property => property.Name, StringComparer.Ordinal)];
        }

        private static bool HasMethod(Type interfaceType, string name)
        {
            return new[] { interfaceType }
                .Concat(interfaceType.GetInterfaces())
                .SelectMany(type => type.GetMethods())
                .Any(method => string.Equals(method.Name, name, StringComparison.Ordinal));
        }
    }
}
