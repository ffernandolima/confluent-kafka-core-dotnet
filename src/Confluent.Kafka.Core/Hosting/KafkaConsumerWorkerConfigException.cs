using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerWorkerConfigException : ValidationException
    {
        private const string ExceptionMessage = $"One or more errors have occurred while validating a '{nameof(KafkaConsumerWorkerConfig)}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public KafkaConsumerWorkerConfigException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? [];
        }
    }
}
