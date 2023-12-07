using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal static class PollyRetryHandlerFactory
    {
        public static IPollyRetryHandler<TKey, TValue> GetOrCreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory = null,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null)
        {
            var retryHandler = serviceProvider?.GetService<IPollyRetryHandler<TKey, TValue>>() ??
                CreateRetryHandler<TKey, TValue>(
                    serviceProvider,
                    loggerFactory,
                    configureOptions);

            return retryHandler;
        }

        public static IPollyRetryHandler<TKey, TValue> CreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory = null,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null)
        {
            var options = PollyRetryHandlerOptionsBuilder.Build(configureOptions);

            var retryHandler = new PollyRetryHandler<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                options);

            return retryHandler;
        }
    }
}
