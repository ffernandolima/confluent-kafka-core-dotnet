using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal sealed class PollyRetryHandler<TKey, TValue> : IRetryHandler<TKey, TValue>
    {
        private static readonly Type DefaultRetryHandlerType = typeof(PollyRetryHandler<TKey, TValue>);

        private readonly ILogger _logger;
        private readonly PolicyBuilder _policyBuilder;
        private readonly PollyRetryHandlerOptions _options;

        public PollyRetryHandler(ILoggerFactory loggerFactory, PollyRetryHandlerOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ValidateAndThrow<PollyRetryHandlerOptionsException>();

            _logger = loggerFactory.CreateLogger(options.EnableLogging, DefaultRetryHandlerType);
            _policyBuilder = Policy.Handle<Exception>(ShouldHandle);
            _options = options;
        }

        public void Handle(
            Action<CancellationToken> executeAction,
            Action<Exception, TimeSpan, int> onRetryAction = null,
            CancellationToken cancellationToken = default)
        {
            if (executeAction is null)
            {
                throw new ArgumentNullException(nameof(executeAction));
            }

            _policyBuilder
                .WaitAndRetry(_options.RetryCount, _options.DelayProvider, _options.Delays, onRetryAction ?? OnRetry)
                .Execute(executeAction, cancellationToken);
        }

        public async Task HandleAsync(
            Func<CancellationToken, Task> executeAction,
            Action<Exception, TimeSpan, int> onRetryAction = null,
            CancellationToken cancellationToken = default)
        {
            if (executeAction is null)
            {
                throw new ArgumentNullException(nameof(executeAction));
            }

            await _policyBuilder
                .WaitAndRetryAsync(_options.RetryCount, _options.DelayProvider, _options.Delays, onRetryAction ?? OnRetry)
                .ExecuteAsync(executeAction, cancellationToken)
                .ConfigureAwait(false);
        }

        private bool ShouldHandle(Exception exception)
        {
            var shouldHandle = _options.RetrySpecification!.IsSatisfiedBy(exception);

            if (!shouldHandle)
            {
                _logger.LogExecutionNotRetriable();
            }

            return shouldHandle;
        }

        private void OnRetry(Exception exception, TimeSpan _, int retryAttempt)
           => _logger.LogRetryExecutionFailure(exception, retryAttempt);
    }
}
