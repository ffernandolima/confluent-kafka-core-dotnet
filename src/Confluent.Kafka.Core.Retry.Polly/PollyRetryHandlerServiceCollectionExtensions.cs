using Confluent.Kafka.Core.Retry.Polly;
using Confluent.Kafka.Core.Retry.Polly.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PollyRetryHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddPollyRetryHandler<TKey, TValue>(
            this IServiceCollection services,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null,
            object handlerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.TryAddKeyedSingleton(
                handlerKey ?? PollyRetryHandlerConstants.PollyRetryHandlerKey,
                (serviceProvider, _) =>
                {
                    var retryHandler = PollyRetryHandlerFactory.CreateRetryHandler<TKey, TValue>(
                        serviceProvider,
                        serviceProvider.GetService<ILoggerFactory>(),
                        configureOptions);

                    return retryHandler;
                });

            return services;
        }
    }
}
