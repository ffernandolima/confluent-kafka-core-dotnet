﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerFactory
    {
        private static readonly Lazy<KafkaProducerFactory> Factory = new(
            () => new KafkaProducerFactory(), isThreadSafe: true);

        public static KafkaProducerFactory Instance => Factory.Value;

        private KafkaProducerFactory()
        { }

        public IKafkaProducer<TKey, TValue> GetOrCreateProducer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey)
        {
            var producerbuilder = serviceProvider?.GetKeyedService<IKafkaProducerBuilder<TKey, TValue>>(producerKey) ??
                new KafkaProducerBuilder<TKey, TValue>()
                    .WithProducerKey(producerKey)
                    .WithConfiguration(
                        configuration ??
                        serviceProvider?.GetService<IConfiguration>())
                    .WithLoggerFactory(
                        loggerFactory ??
                        serviceProvider?.GetService<ILoggerFactory>())
                    .WithServiceProvider(serviceProvider);

            configureProducer?.Invoke(producerbuilder);

#if NETSTANDARD2_0_OR_GREATER
            var producer = producerbuilder.Build().ToKafkaProducer();
#else
            var producer = producerbuilder.Build();
#endif
            return producer;
        }
    }
}
