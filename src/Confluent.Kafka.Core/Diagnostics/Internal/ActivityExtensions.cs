using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal static class ActivityExtensions
    {
        public static ActivityContext GetContextOrDefault(this Activity activity)
        {
            var activityContext = activity?.Context ?? default;

            return activityContext;
        }

        public static IEnumerable<KeyValuePair<string, string>> GetBaggageOrEmpty(this Activity activity)
        {
            var baggage = activity?.Baggage ?? [];

            return baggage;
        }

        public static Activity SetBaggageItems(this Activity activity, IEnumerable<KeyValuePair<string, string>> baggageItems)
        {
            if (baggageItems is not null && baggageItems.Any())
            {
                foreach (var baggageItem in baggageItems)
                {
                    activity?.SetBaggage(baggageItem.Key, baggageItem.Value);
                }
            }

            return activity;
        }

        public static Activity AddEvent(this Activity activity, string name, DateTimeOffset timestamp = default, ActivityTagsCollection tags = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var activityEvent = new ActivityEvent(name, timestamp, tags);

                activity?.AddEvent(activityEvent);
            }

            return activity;
        }
    }
}
