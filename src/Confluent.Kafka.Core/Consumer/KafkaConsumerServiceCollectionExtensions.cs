﻿using Confluent.Kafka.Core.Consumer;
#if NETSTANDARD2_0_OR_GREATER
using Confluent.Kafka.Core.Consumer.Internal;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaConsumerServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumer<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureConsumer is null)
            {
                throw new ArgumentNullException(nameof(configureConsumer), $"{nameof(configureConsumer)} cannot be null.");
            }

            services.AddKafkaDiagnostics();

            services.TryAddKeyedSingleton(consumerKey, (serviceProvider, _) =>
            {
                var builder = new KafkaConsumerBuilder<TKey, TValue>()
                    .WithConsumerKey(consumerKey)
                    .WithConfiguration(serviceProvider.GetService<IConfiguration>())
                    .WithLoggerFactory(serviceProvider.GetService<ILoggerFactory>())
                    .WithServiceProvider(serviceProvider);

                configureConsumer.Invoke(serviceProvider, builder);

                return builder;
            });

            services.TryAddKeyedSingleton(consumerKey, (serviceProvider, _) =>
            {
                var builder = serviceProvider.GetRequiredKeyedService<IKafkaConsumerBuilder<TKey, TValue>>(consumerKey);

#if NETSTANDARD2_0_OR_GREATER
                var consumer = builder.Build().ToKafkaConsumer();
#else
                var consumer = builder.Build();
#endif
                return consumer;
            });

            services.AddKafkaConsumerHandlerFactory<TKey, TValue>((serviceProvider, builder) =>
            {
                var consumerBuilder = serviceProvider.GetRequiredKeyedService<IKafkaConsumerBuilder<TKey, TValue>>(consumerKey);

                builder.WithEnableLogging(consumerBuilder.ConsumerConfig!.EnableLogging);
            },
            consumerKey);

            return services;
        }
    }
}
