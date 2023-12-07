using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal sealed class PollyRetryHandler<TKey, TValue> : IPollyRetryHandler<TKey, TValue>
    {
        private static readonly Type DefaultRetryHandlerType = typeof(PollyRetryHandler<TKey, TValue>);

        private readonly ILogger _logger;
        private readonly PolicyBuilder _policyBuilder;
        private readonly PollyRetryHandlerOptions _options;

        private Func<Exception, bool> _cachedExceptionFilter;
        private IEnumerable<Type> _cachedExceptionTypeFilters;

        public PollyRetryHandler(ILoggerFactory loggerFactory, PollyRetryHandlerOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            options.ValidateAndThrow<PollyRetryHandlerOptionsException>();

            _logger = loggerFactory.CreateLogger(options.EnableLogging, DefaultRetryHandlerType);
            _policyBuilder = Policy.Handle<Exception>(ShouldHandle);
            _options = options;
        }

        public void TryHandle(
            Action<CancellationToken> executeAction,
            Action<Exception, TimeSpan, int> onRetryAction = null,
            CancellationToken cancellationToken = default)
        {
            if (executeAction is null)
            {
                throw new ArgumentNullException(nameof(executeAction), $"{nameof(executeAction)} cannot be null.");
            }

            _policyBuilder
                .WaitAndRetry(_options.RetryCount, _options.DelayProvider, _options.Delays, onRetryAction ?? OnRetry)
                .Execute(executeAction, cancellationToken);
        }
        public async Task TryHandleAsync(
        Func<CancellationToken, Task> executeAction,
            Action<Exception, TimeSpan, int> onRetryAction = null,
            CancellationToken cancellationToken = default)
        {
            if (executeAction is null)
            {
                throw new ArgumentNullException(nameof(executeAction), $"{nameof(executeAction)} cannot be null.");
            }

            await _policyBuilder
                .WaitAndRetryAsync(_options.RetryCount, _options.DelayProvider, _options.Delays, onRetryAction ?? OnRetry)
                .ExecuteAsync(executeAction, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        private bool ShouldHandle(Exception exception)
        {
            _cachedExceptionFilter ??= _options.ExceptionFilter is null
                ? exception => true
                : _options.ExceptionFilter;

            _cachedExceptionTypeFilters ??= _options.ExceptionTypeFilters is null
                ? Enumerable.Empty<Type>()
                : _options.ExceptionTypeFilters
                          .Select(typeName => Type.GetType(typeName, throwOnError: false, ignoreCase: true))
                          .Where(type => type is not null);

            var shouldHandle = _cachedExceptionFilter.Invoke(exception) && !_cachedExceptionTypeFilters.Contains(exception.GetType());

            if (!shouldHandle)
            {
                _logger.LogExecutionNotRetriable();
            }

            return shouldHandle;
        }

        private void OnRetry(Exception exception, TimeSpan _, int retryAttempt)
           => _logger.LogRetryExecutionFailed(exception, retryAttempt);
    }
}
