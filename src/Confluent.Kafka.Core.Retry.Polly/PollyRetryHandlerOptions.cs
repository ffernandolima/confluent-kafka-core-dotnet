using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

namespace Confluent.Kafka.Core.Retry.Polly
{
    public sealed class PollyRetryHandlerOptions : IValidatableObject
    {
        private IEnumerable<TimeSpan> _delays;
        private Func<int, TimeSpan> _delayProvider;
        private RetrySpecification _retrySpecification;

        public int RetryCount { get; set; } = 1;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.Zero;
        public IEnumerable<TimeSpan> Delays { get => _delays; set => SetDelays(delays: value); }
        public Func<int, TimeSpan> DelayProvider { get => _delayProvider; set => SetDelayProvider(delayProvider: value); }
        public string[] ExceptionTypeFilters { get; set; }
        public Func<Exception, bool> ExceptionFilter { get; set; }
        public bool EnableLogging { get; set; } = true;
        public RetrySpecification RetrySpecification =>
            _retrySpecification ??= RetrySpecification.Create(ExceptionFilter, ExceptionTypeFilters);

        public PollyRetryHandlerOptions()
        {
            DelayProvider = retryAttempt => RetryDelay;
        }

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var options = validationContext?.ObjectInstance as PollyRetryHandlerOptions ?? this;

            if (options.RetryCount < 0)
            {
                yield return new ValidationResult(
                    $"{nameof(options.RetryCount)} cannot be less than zero.",
                    [nameof(options.RetryCount)]);
            }

            if (options.RetryDelay == Timeout.InfiniteTimeSpan)
            {
                yield return new ValidationResult(
                    $"{nameof(options.RetryDelay)} cannot be infinite.",
                    [nameof(options.RetryDelay)]);
            }

            switch (options.DelayProvider, options.Delays)
            {
                case (DelayProvider: null, Delays: null):
                    {
                        yield return new ValidationResult(
                            $"Neither {nameof(options.DelayProvider)} nor {nameof(options.Delays)} has been set up.",
                            [nameof(options.DelayProvider), nameof(options.Delays)]);
                    }
                    break;

                case (DelayProvider: not null, Delays: not null):
                    {
                        yield return new ValidationResult(
                            $"Both {nameof(options.DelayProvider)} and {nameof(options.Delays)} have been set up.",
                            [nameof(options.DelayProvider), nameof(options.Delays)]);
                    }
                    break;
            }

            if (options.Delays is not null && options.Delays.Any(delay => delay == Timeout.InfiniteTimeSpan))
            {
                yield return new ValidationResult(
                    $"{nameof(options.Delays)} cannot contain any infinite delay.",
                    [nameof(options.Delays)]);
            }
        }

        #endregion IValidatableObject Members

        private void SetDelayProvider(Func<int, TimeSpan> delayProvider)
        {
            if (delayProvider is null)
            {
                _delayProvider = null;
            }
            else
            {
                _delayProvider = retryAttempt =>
                {
                    var delay = delayProvider.Invoke(retryAttempt);

                    if (delay == Timeout.InfiniteTimeSpan)
                    {
                        throw new InvalidOperationException("Delay cannot be infinite.");
                    }

                    return delay;
                };

                _delays = null;
            }
        }

        private void SetDelays(IEnumerable<TimeSpan> delays)
        {
            if (delays is null)
            {
                _delays = null;
            }
            else
            {
                _delays = delays;
                _delayProvider = null;
            }
        }
    }
}
