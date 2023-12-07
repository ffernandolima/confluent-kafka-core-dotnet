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

                var options = builder.ToOptions();

                return options;
            });

            services.AddSingleton(provider =>
            {
                var builder = provider.GetRequiredService<IKafkaProducerBuilder<TKey, TValue>>();

                var producer = builder.Build();

                return producer;
            });

            services.AddSingleton<IKafkaProducerHandlerFactory<TKey, TValue>>(provider =>
            {
                var producerOptions = provider.GetRequiredService<IKafkaProducerOptions<TKey, TValue>>();

                var loggerFactory = producerOptions.LoggerFactory ?? provider.GetService<ILoggerFactory>();

                var producerHandlerFactoryOptions = new KafkaProducerHandlerFactoryOptions
                {
                    EnableLogging = producerOptions.ProducerConfig!.EnableLogging
                };

                return new KafkaProducerHandlerFactory<TKey, TValue>(loggerFactory, producerHandlerFactoryOptions);
            });

            return services;
        }
    }
}
