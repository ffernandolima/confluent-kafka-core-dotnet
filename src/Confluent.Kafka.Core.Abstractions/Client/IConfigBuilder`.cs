using System.Collections.Generic;

namespace Confluent.Kafka.Core.Client
{
    public interface IConfigBuilder<TBuilder> where TBuilder : IConfigBuilder<TBuilder>
    {
        TBuilder WithCancellationDelayMaxMs(int cancellationDelayMaxMs);

        TBuilder WithConfigProperty(KeyValuePair<string, string> configProperty);

        TBuilder WithConfigProperty(string configPropertyKey, string configPropertyValue);
    }
}
