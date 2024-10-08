using Confluent.Kafka.Core.Specifications;
using System;
using System.Linq;

namespace Confluent.Kafka.Core.Retry
{
    public sealed class RetrySpecification : ExpressionSpecification<Exception>
    {
        private RetrySpecification(Func<Exception, bool> exceptionFilter, Type[] exceptionTypeFilters)
            : base(
                  exception =>
                    exceptionFilter.Invoke(exception) && !exceptionTypeFilters.Contains(exception.GetType()))
        { }

        public static RetrySpecification Create(Func<Exception, bool> exceptionFilter, string[] exceptionTypeFilters)
        {
            var retrySpecification = new RetrySpecification(
                exceptionFilter is null
                    ? exception => true
                    : exceptionFilter,
                exceptionTypeFilters is null
                    ? []
                    : exceptionTypeFilters
                        .Select(typeName => Type.GetType(typeName, throwOnError: false, ignoreCase: true))
                        .Where(type => type is not null)
                        .ToArray());

            return retrySpecification;
        }

        protected override bool Evaluate(Exception source)
        {
            if (source is null)
            {
                return false;
            }

            return base.Evaluate(source);
        }
    }
}
