using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Producer
{
    public sealed class KafkaProducerConfigException : ValidationException
    {
        private const string ExceptionMessage = $"One or more errors have occurred while validating a '{nameof(KafkaProducerConfig)}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public KafkaProducerConfigException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? [];
        }
    }
}
