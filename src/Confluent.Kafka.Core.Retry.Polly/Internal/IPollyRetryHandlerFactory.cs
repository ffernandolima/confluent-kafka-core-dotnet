using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal interface IPollyRetryHandlerFactory
    {
        IRetryHandler<TKey, TValue> GetOrCreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions,
            object handlerKey);

        IRetryHandler<TKey, TValue> CreateRetryHandler<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IPollyRetryHandlerOptionsBuilder> configureOptions);
    }
}
