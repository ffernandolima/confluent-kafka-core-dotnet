using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics
{
    public interface IContextPropagator
    {
        void InjectContext(Activity activity, IDictionary<string, string> carrier);

        IPropagationContext ExtractContext(IDictionary<string, string> carrier);
    }
}
