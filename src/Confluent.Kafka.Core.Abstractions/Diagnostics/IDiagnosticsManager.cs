using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics
{
    public interface IDiagnosticsManager : IContextPropagator, IKafkaActivityEnricher
    {
        Activity StartActivity(string activityName, ActivityKind activityKind, IPropagationContext propagationContext);

        Activity StartProducerActivity(string activityName, IDictionary<string, string> carrier);

        Activity StartConsumerActivity(string activityName, IDictionary<string, string> carrier);
    }
}
