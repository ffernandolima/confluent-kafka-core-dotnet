using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Internal
{
    internal static class ExceptionExtensions
    {
        public static IEnumerable<Exception> GetInnerExceptions(this Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var exceptions = new List<Exception>();

            var exceptionInternal = exception;

            do
            {
                exceptions.Add(exceptionInternal);

                exceptionInternal = exceptionInternal.InnerException;
            }
            while (exceptionInternal is not null);

            exceptions.Reverse();

            return exceptions;
        }
    }
}
