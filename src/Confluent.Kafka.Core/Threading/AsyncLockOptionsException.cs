using Confluent.Kafka.Core.Threading.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Threading
{
    public sealed class AsyncLockOptionsException : ValidationException
    {
        private const string ExceptionMessage = $"One or more errors have occurred while validating an '{nameof(AsyncLockOptions)}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public AsyncLockOptionsException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? [];
        }
    }
}
