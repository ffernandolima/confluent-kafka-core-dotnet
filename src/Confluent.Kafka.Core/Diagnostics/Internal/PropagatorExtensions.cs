using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal static class PropagatorExtensions
    {
        public static void Inject(this DistributedContextPropagator propagator, Activity activity, IDictionary<string, string> carrier)
        {
            if (propagator is null)
            {
                throw new ArgumentNullException(nameof(propagator));
            }

            propagator.Inject(
                activity,
                carrier,
                (carrier, fieldName, fieldValue) =>
                {
                    if (carrier is IDictionary<string, string> typedCarrier)
                    {
                        typedCarrier.Remove(fieldName);
                        typedCarrier.Add(fieldName, fieldValue);
                    }
                });
        }

        public static IPropagationContext Extract(this DistributedContextPropagator propagator, IDictionary<string, string> carrier)
        {
            if (propagator is null)
            {
                throw new ArgumentNullException(nameof(propagator));
            }

            propagator.ExtractTraceIdAndState(
                carrier,
                (object carrier, string fieldName, out string fieldValue, out IEnumerable<string> fieldValues) =>
                {
                    fieldValue = null;
                    fieldValues = null;

                    if (carrier is IDictionary<string, string> typedCarrier)
                    {
                        typedCarrier.TryGetValue(fieldName, out fieldValue);
                    }
                },
                out var traceId,
                out var traceState);

            var baggageItems = propagator.ExtractBaggage(
                carrier,
                (object carrier, string fieldName, out string fieldValue, out IEnumerable<string> fieldValues) =>
                {
                    fieldValue = null;
                    fieldValues = null;

                    if (carrier is IDictionary<string, string> typedCarrier)
                    {
                        typedCarrier.TryGetValue(fieldName, out fieldValue);
                    }
                });

            ActivityContext.TryParse(traceId, traceState, out var activityContext);

            var propagationContext = new PropagationContext(activityContext, baggageItems);

            return propagationContext;
        }
    }
}
