using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class AsyncLockOptions : IValidatableObject
    {
        public int MaxDegreeOfParallelism { get; set; } = 1;
        public bool HandleLockByKey { get; set; }
        public Func<AsyncLockContext, object> LockKeyHandler { get; set; }

        public static IAsyncLockOptionsBuilder CreateBuilder() => new AsyncLockOptionsBuilder();

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var options = validationContext?.ObjectInstance as AsyncLockOptions ?? this;

            if (options.MaxDegreeOfParallelism <= 0)
            {
                yield return new ValidationResult(
                    $"{nameof(options.MaxDegreeOfParallelism)} cannot be less than or equal to zero.",
                    [nameof(options.MaxDegreeOfParallelism)]);
            }

            if (options.HandleLockByKey && options.LockKeyHandler is null)
            {
                yield return new ValidationResult(
                    $"{nameof(options.LockKeyHandler)} cannot be null when {nameof(options.HandleLockByKey)} is true.",
                    [nameof(options.LockKeyHandler), nameof(options.HandleLockByKey)]);
            }
        }

        #endregion IValidatableObject Members
    }
}
