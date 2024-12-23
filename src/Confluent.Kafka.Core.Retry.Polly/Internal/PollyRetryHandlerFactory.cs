﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal sealed class PollyRetryHandlerFactory : IPollyRetryHandlerFactory
    {
        private static readonly Lazy<PollyRetryHandlerFactory> Factory = new(
           () => new PollyRetryHandlerFactory(), isThreadSafe: true);

        public static PollyRetryHandlerFactory Instance => Factory.Value;

        private PollyRetryHandlerFactory()
        { }

        public IRetryHandler<TKey, TValue> GetOrCreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions,
            object handlerKey)
        {
            var retryHandler = serviceProvider?.GetKeyedService<IRetryHandler<TKey, TValue>>(
                handlerKey ?? PollyRetryHandlerConstants.PollyRetryHandlerKey) ??
                CreateRetryHandler<TKey, TValue>(
                    serviceProvider,
                    configuration,
                    loggerFactory,
                    (_, builder) => configureOptions?.Invoke(builder));

            return retryHandler;
        }

        public IRetryHandler<TKey, TValue> CreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IPollyRetryHandlerOptionsBuilder> configureOptions)
        {
            var options = PollyRetryHandlerOptionsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureOptions);

            var retryHandler = new PollyRetryHandler<TKey, TValue>(
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                options);

            return retryHandler;
        }
    }
}
