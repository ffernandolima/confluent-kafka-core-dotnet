using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal static class PollyRetryHandlerFactory
    {
        public static IRetryHandler<TKey, TValue> GetOrCreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions,
            object handlerKey)
        {
            var retryHandler = serviceProvider?.GetKeyedService<IRetryHandler<TKey, TValue>>(
                handlerKey ?? PollyRetryHandlerConstants.PollyRetryHandlerKey) ??
                CreateRetryHandler<TKey, TValue>(serviceProvider, loggerFactory, configureOptions);

            return retryHandler;
        }

        public static IRetryHandler<TKey, TValue> CreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions)
        {
            var options = PollyRetryHandlerOptionsBuilder.Build(configureOptions);

            var retryHandler = new PollyRetryHandler<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                options);

            return retryHandler;
        }
    }
}
