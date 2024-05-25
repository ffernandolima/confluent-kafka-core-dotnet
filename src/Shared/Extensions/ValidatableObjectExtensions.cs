using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Internal
{
    internal static class ValidatableObjectExtensions
    {
        public static void ValidateAndThrow<TException>(
            this IValidatableObject validatableObject,
            ValidationContext validationContext = null,
            bool validateAllProperties = false)
                where TException : ValidationException
        {
            if (validatableObject is null)
            {
                throw new ArgumentNullException(nameof(validatableObject), $"{nameof(validatableObject)} cannot be null.");
            }

            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(
                validatableObject,
                validationContext ?? new ValidationContext(validatableObject),
                validationResults,
                validateAllProperties))
            {
                throw (TException)Activator.CreateInstance(typeof(TException), validationResults);
            }
        }
    }
}
