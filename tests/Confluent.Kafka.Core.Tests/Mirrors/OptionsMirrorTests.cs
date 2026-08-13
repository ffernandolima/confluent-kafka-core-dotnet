using Confluent.Kafka.Core.Retry.Polly;
using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Mirrors
{
    /// <summary>
    /// These options types are declared by this library rather than by a package, so a dependency
    /// upgrade cannot desync them. They are covered because adding a property without the matching
    /// <c>With*</c> leaves it unreachable through the fluent API, which nothing else reports.
    /// </summary>
    public sealed class OptionsMirrorTests
    {
        [Fact]
        public void PollyRetryHandlerOptions_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(PollyRetryHandlerOptions), typeof(IPollyRetryHandlerOptionsBuilder));
        }

        [Fact]
        public void ProtobufNetSerializerOptions_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(ProtobufNetSerializerOptions), typeof(IProtobufNetSerializerOptionsBuilder));
        }
    }
}
