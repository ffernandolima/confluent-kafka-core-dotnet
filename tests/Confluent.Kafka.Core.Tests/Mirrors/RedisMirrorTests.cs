using Confluent.Kafka.Core.Idempotency.Redis;
using StackExchange.Redis;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Mirrors
{
    /// <summary>
    /// IConfigurationOptionsBuilder is a hand-maintained mirror of ConfigurationOptions, so bumping
    /// StackExchange.Redis can leave a new upstream property unreachable through the fluent API.
    /// This test catches that.
    /// </summary>
    public sealed class RedisMirrorTests
    {
        // CertificateSelection and CertificateValidation are events rather than properties, so the scan skips them;
        // the builder exposes them as With* methods that subscribe.
        [Fact]
        public void ConfigurationOptions_IsFullyMirrored()
        {
            MirrorAssert.IsComplete(typeof(ConfigurationOptions), typeof(IConfigurationOptionsBuilder));
        }
    }
}
