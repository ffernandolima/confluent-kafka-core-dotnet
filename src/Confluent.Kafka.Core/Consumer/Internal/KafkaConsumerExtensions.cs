using Confluent.Kafka.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class KafkaConsumerExtensions
    {
        public static void ValidateAndThrow(this IKafkaConsumer<byte[], KafkaMetadataMessage> consumer, string suffix)
        {
            if (consumer is null)
            {
                throw new ArgumentNullException(nameof(consumer));
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                throw new ArgumentException($"{nameof(suffix)} cannot be null or whitespace.", nameof(suffix));
            }

            if (consumer.Subscription!.Count > 0 || consumer.Assignment!.Count > 0)
            {
                List<string> memberNames = null;

                if (consumer.Subscription!.Any(topic => string.IsNullOrWhiteSpace(topic) || !topic.EndsWith(suffix)))
                {
                    memberNames ??= [];
                    memberNames.Add(nameof(consumer.Subscription));
                }

                if (consumer.Assignment!.Any(assignment => string.IsNullOrWhiteSpace(assignment.Topic) || !assignment.Topic!.EndsWith(suffix)))
                {
                    memberNames ??= [];
                    memberNames.Add(nameof(consumer.Assignment));
                }

                if (memberNames is not null && memberNames.Count > 0)
                {
                    throw new KafkaConsumerConfigException(
                        [
                            new ValidationResult(
                            $"Topic(s) from subscription and/or assignment must end with '{suffix}' suffix.",
                            memberNames)
                        ]);
                }
            }
        }
    }
}
