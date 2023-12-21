using Confluent.Kafka.Core.Retry.Polly;
using Confluent.Kafka.Core.Retry.Polly.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PollyRetryHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddPollyMessageRetryHandler<TKey, TValue>(
            this IServiceCollection services,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.TryAddSingleton(serviceProvider =>
            {
                var retryHandler = PollyRetryHandlerFactory.CreateRetryHandler<TKey, TValue>(
                    serviceProvider,
                    configureOptions: configureOptions);

                return retryHandler;
            });

            return services;
        }
    }
}
