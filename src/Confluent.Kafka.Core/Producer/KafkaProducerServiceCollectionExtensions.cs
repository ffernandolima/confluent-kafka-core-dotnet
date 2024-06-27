﻿using Confluent.Kafka.Core.Producer;
#if NETSTANDARD2_0_OR_GREATER
using Confluent.Kafka.Core.Producer.Internal;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaProducerServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaProducer<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureProducer is null)
            {
                throw new ArgumentNullException(nameof(configureProducer), $"{nameof(configureProducer)} cannot be null.");
            }

            services.AddKafkaDiagnostics();

            services.TryAddKeyedSingleton(producerKey, (serviceProvider, _) =>
            {
                var builder = new KafkaProducerBuilder<TKey, TValue>()
                    .WithProducerKey(producerKey)
                    .WithConfiguration(serviceProvider.GetService<IConfiguration>())
                    .WithLoggerFactory(serviceProvider.GetService<ILoggerFactory>())
                    .WithServiceProvider(serviceProvider);

                configureProducer.Invoke(serviceProvider, builder);

                return builder;
            });

            services.TryAddKeyedSingleton(producerKey, (serviceProvider, _) =>
            {
                var builder = serviceProvider.GetRequiredKeyedService<IKafkaProducerBuilder<TKey, TValue>>(producerKey);

#if NETSTANDARD2_0_OR_GREATER
                var producer = builder.Build().ToKafkaProducer();
#else
                var producer = builder.Build();
#endif
                return producer;
            });

            services.AddKafkaProducerHandlerFactory<TKey, TValue>((serviceProvider, builder) =>
            {
                var producerBuilder = serviceProvider.GetRequiredKeyedService<IKafkaProducerBuilder<TKey, TValue>>(producerKey);

                builder.WithEnableLogging(producerBuilder.ProducerConfig!.EnableLogging);
            },
            producerKey);

            return services;
        }
    }
}
