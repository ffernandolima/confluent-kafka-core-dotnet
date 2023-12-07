using System.Collections.Generic;

namespace Confluent.Kafka.Core.Client
{
    public interface IConfig : IEnumerable<KeyValuePair<string, string>>
    {
        public int CancellationDelayMaxMs { get; }

        public string Get(string key);
    }
}
