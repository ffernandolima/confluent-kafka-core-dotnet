using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer
{
    public sealed class KafkaConsumerConfigException : ValidationException
    {
        private const string ExceptionMessage = $"One or more errors have occurred while validating a '{nameof(KafkaConsumerConfig)}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public KafkaConsumerConfigException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? Enumerable.Empty<ValidationResult>();
        }
    }
}
