using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Models
{
    public sealed class TopicPartitionException : ValidationException
    {
        private const string ExceptionMessage = $"One or more errors have occurred while validating a '{nameof(TopicPartition)}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public TopicPartitionException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? [];
        }
    }
}
