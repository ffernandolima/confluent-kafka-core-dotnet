using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public sealed class RedisIdempotencyHandlerOptions<TKey, TValue> : IValidatableObject
    {
        public string GroupId { get; set; }
        public string ConsumerName { get; set; }
        public TimeSpan ExpirationInterval { get; set; } = TimeSpan.FromDays(7);
        public Func<TValue, string> MessageIdHandler { get; set; }
        public bool EnableLogging { get; set; } = true;

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var options = validationContext?.ObjectInstance as RedisIdempotencyHandlerOptions<TKey, TValue> ?? this;

            if (string.IsNullOrWhiteSpace(options.GroupId))
            {
                yield return new ValidationResult(
                    $"{nameof(options.GroupId)} cannot be null or whitespace.",
                    new[] { nameof(options.GroupId) });
            }

            if (string.IsNullOrWhiteSpace(options.ConsumerName))
            {
                yield return new ValidationResult(
                    $"{nameof(options.ConsumerName)} cannot be null or whitespace.",
                    new[] { nameof(options.ConsumerName) });
            }

            if (options.ExpirationInterval == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(options.ExpirationInterval)} cannot be infinite.",
                    new[] { nameof(options.ExpirationInterval) });
            }

            if (options.MessageIdHandler is null)
            {
                yield return new ValidationResult(
                    $"{nameof(options.MessageIdHandler)} cannot be null.",
                    new[] { nameof(options.MessageIdHandler) });
            }
        }

        #endregion IValidatableObject Members
    }
}
