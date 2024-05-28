using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Confluent.Kafka.Core.Retry.Polly
{
    public sealed class PollyRetryHandlerOptionsException : ValidationException
    {
        private const string ExceptionMessage = $"One or more errors have occurred while validating a '{nameof(PollyRetryHandlerOptions)}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public PollyRetryHandlerOptionsException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? [];
        }
    }
}
