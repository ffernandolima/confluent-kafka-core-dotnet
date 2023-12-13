using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaProducerServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaProducer<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer)
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

            services.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetService<ILoggerFactory>();

                var builder = new KafkaProducerBuilder<TKey, TValue>()
                    .WithServiceProvider(provider)
                    .WithLoggerFactory(loggerFactory);

                configureProducer.Invoke(provider, builder);

                return builder;
            });

            services.AddSingleton(provider =>
            {
                var builder = provider.GetRequiredService<IKafkaProducerBuilder<TKey, TValue>>();

                var producer = builder.Build();

                return producer;
            });

            services.AddSingleton<IKafkaProducerHandlerFactory<TKey, TValue>>(provider =>
            {
                var builder = provider.GetRequiredService<IKafkaProducerBuilder<TKey, TValue>>();

                var loggerFactory = builder.LoggerFactory ?? provider.GetService<ILoggerFactory>();

                var options = new KafkaProducerHandlerFactoryOptions
                {
                    EnableLogging = builder.ProducerConfig!.EnableLogging
                };

                return new KafkaProducerHandlerFactory<TKey, TValue>(loggerFactory, options);
            });

            return services;
        }
    }
}
