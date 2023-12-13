using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Consumer.Internal;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaConsumerServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumer<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer)
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

            services.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetService<ILoggerFactory>();

                var builder = new KafkaConsumerBuilder<TKey, TValue>()
                    .WithServiceProvider(provider)
                    .WithLoggerFactory(loggerFactory);

                configureConsumer.Invoke(provider, builder);

                return builder;
            });

            services.AddSingleton(provider =>
            {
                var builder = provider.GetRequiredService<IKafkaConsumerBuilder<TKey, TValue>>();

                var consumer = builder.Build();

                return consumer;
            });

            services.AddSingleton<IKafkaConsumerHandlerFactory<TKey, TValue>>(provider =>
            {
                var builder = provider.GetRequiredService<KafkaConsumerBuilder<TKey, TValue>>();

                var loggerFactory = builder.LoggerFactory ?? provider.GetService<ILoggerFactory>();

                var options = new KafkaConsumerHandlerFactoryOptions
                {
                    EnableLogging = builder.ConsumerConfig!.EnableLogging
                };

                return new KafkaConsumerHandlerFactory<TKey, TValue>(loggerFactory, options);
            });

            return services;
        }
    }
}
