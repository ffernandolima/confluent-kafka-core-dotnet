using Confluent.Kafka.Core.Retry.Polly;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class PollyRetryHandlerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddPollyRetryHandler<TKey, TValue>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, IPollyRetryHandlerOptionsBuilder> configureOptions = null,
            object handlerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services!.AddPollyRetryHandler<TKey, TValue>(configureOptions, handlerKey);

            return builder;
        }
    }
}
