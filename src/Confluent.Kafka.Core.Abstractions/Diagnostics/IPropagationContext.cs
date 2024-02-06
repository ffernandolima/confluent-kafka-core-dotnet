using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics
{
    public interface IPropagationContext
    {
        ActivityContext ActivityContext { get; }

        IEnumerable<KeyValuePair<string, string>> Baggage { get; }
    }
}
