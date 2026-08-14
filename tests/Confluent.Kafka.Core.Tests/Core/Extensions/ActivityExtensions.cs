using Confluent.Kafka.Core.Diagnostics.Internal;
using System.Diagnostics;
using System.Linq;

namespace Confluent.Kafka.Core.Tests.Core.Extensions
{
    public static class ActivityExtensions
    {
        public static bool HasTopic(this Activity activity, params string[] topics)
        {
            var topicKey = SemanticConventions.Messaging.DestinationName;

            var topicKvp = activity?.Tags.SingleOrDefault(tag => tag.Key == topicKey) ?? default;

            var hasTopic = topics.Contains(topicKvp.Value);

            return hasTopic;
        }

        public static bool IsProducerKind(this Activity activity)
        {
            var isProducerKind = activity is not null && activity.Kind == ActivityKind.Producer;

            return isProducerKind;
        }

        /// <summary>
        /// Matches both consumer-side kinds: 
        /// receive is Client and processing is Consumer, per the messaging conventions. 
        /// Tests use this to mean "a consumer-side span for this topic".
        /// </summary>
        public static bool IsConsumerKind(this Activity activity)
        {
            var isConsumerKind = activity is not null && activity.Kind is ActivityKind.Consumer or ActivityKind.Client;

            return isConsumerKind;
        }
    }
}
