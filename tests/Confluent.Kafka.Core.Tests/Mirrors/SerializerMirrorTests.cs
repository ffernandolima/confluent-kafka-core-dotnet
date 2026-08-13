using Confluent.Kafka.Core.Serialization.JsonCore;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Newtonsoft.Json;
using System.Text.Json;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Mirrors
{
    /// <summary>
    /// The serializer settings builders are hand-maintained mirrors and not enforced by the compiler,
    /// so bumping System.Text.Json or Newtonsoft.Json can leave a new upstream property unreachable
    /// through the fluent API. These tests catch that.
    /// </summary>
    public sealed class SerializerMirrorTests
    {
        // JsonSerializerOptions.Converters and TypeInfoResolverChain are read-only collections, so the
        // property scan skips them; the builder exposes them as mutating With* methods instead.
        [Fact]
        public void JsonSerializerOptions_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(JsonSerializerOptions), typeof(IJsonSerializerOptionsBuilder));
        }

        [Fact]
        public void JsonSerializerSettings_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(JsonSerializerSettings), typeof(IJsonSerializerSettingsBuilder));
        }
    }
}
