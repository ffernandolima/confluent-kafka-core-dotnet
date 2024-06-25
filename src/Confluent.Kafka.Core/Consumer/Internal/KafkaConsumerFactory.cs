﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static class KafkaConsumerFactory
    {
        public static IKafkaConsumer<TKey, TValue> GetOrCreateConsumer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey)
        {
            var consumerbuilder = serviceProvider?.GetKeyedService<IKafkaConsumerBuilder<TKey, TValue>>(consumerKey) ??
                new KafkaConsumerBuilder<TKey, TValue>()
                    .WithConsumerKey(consumerKey)
                    .WithConfiguration(
                        configuration ??
                        serviceProvider?.GetService<IConfiguration>())
                    .WithLoggerFactory(
                        loggerFactory ??
                        serviceProvider?.GetService<ILoggerFactory>())
                    .WithServiceProvider(serviceProvider);

            configureConsumer?.Invoke(consumerbuilder);

#if NETSTANDARD2_0_OR_GREATER
            var consumer = consumerbuilder.Build().ToKafkaConsumer();
#else
            var consumer = consumerbuilder.Build();
#endif
            return consumer;
        }
    }
}