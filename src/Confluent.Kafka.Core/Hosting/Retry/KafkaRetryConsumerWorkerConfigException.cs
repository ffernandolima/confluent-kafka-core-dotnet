using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public sealed class KafkaRetryConsumerWorkerConfigException : ValidationException
    {
        private const string ExceptionMessage = $"One or more errors have occurred while validating a '{nameof(KafkaRetryConsumerWorkerConfig)}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public KafkaRetryConsumerWorkerConfigException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? [];
        }
    }
}
