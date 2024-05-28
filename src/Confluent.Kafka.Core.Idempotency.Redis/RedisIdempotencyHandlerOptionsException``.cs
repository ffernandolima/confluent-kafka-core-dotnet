using Confluent.Kafka.Core.Idempotency.Redis;
using Confluent.Kafka.Core.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Retry.Polly
{
    public sealed class RedisIdempotencyHandlerOptionsException<TKey, TValue> : ValidationException
    {
        private static readonly string ExceptionMessage =
            $"One or more errors have occurred while validating a '{typeof(RedisIdempotencyHandlerOptions<TKey, TValue>).ExtractTypeName()}' instance.";

        public IEnumerable<ValidationResult> Results { get; }

        public RedisIdempotencyHandlerOptionsException(IEnumerable<ValidationResult> results)
            : base(ExceptionMessage)
        {
            Results = results ?? [];
        }
    }
}
