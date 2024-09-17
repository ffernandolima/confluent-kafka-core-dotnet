using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Tests.Core.Diagnostics
{
    internal static class KafkaActivityListener
    {
        public static ActivityListener StartListening(Action<Activity> onListen)
        {
            static bool ShouldListenTo(ActivitySource source) => source.Name == "Confluent.Kafka.Core";

            var activityListener = new ActivityListener
            {
                ShouldListenTo = ShouldListenTo,
                SampleUsingParentId = (ref ActivityCreationOptions<string> activityOptions) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> activityOptions) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => { },
                ActivityStopped = activity => { onListen?.Invoke(activity); }
            };

            ActivitySource.AddActivityListener(activityListener);

            return activityListener;
        }
    }
}
