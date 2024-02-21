using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal static class TopicPartitionExtensions
    {
        public static void ValidateAndThrow(this TopicPartition topicPartition)
        {
            if (topicPartition is null)
            {
                throw new ArgumentNullException(nameof(topicPartition), $"{nameof(topicPartition)} cannot be null.");
            }

            List<ValidationResult> validationResults = null;

            if (string.IsNullOrWhiteSpace(topicPartition.Topic))
            {
                validationResults ??= new List<ValidationResult>();

                validationResults.Add(
                    new ValidationResult(
                        $"{nameof(topicPartition.Topic)} cannot be null or whitespace.",
                        new[] { nameof(topicPartition.Topic) }));
            }

            if (topicPartition.Partition < Partition.Any)
            {
                validationResults ??= new List<ValidationResult>();

                validationResults.Add(
                    new ValidationResult(
                        $"{nameof(topicPartition.Partition)} cannot be lower than {Partition.Any.Value}.",
                        new[] { nameof(topicPartition.Partition) }));
            }

            if (validationResults is not null && validationResults.Count > 0)
            {
                throw new TopicPartitionException(validationResults);
            }
        }
    }
}
