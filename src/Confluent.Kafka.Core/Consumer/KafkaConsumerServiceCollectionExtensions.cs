using Confluent.Kafka.Core.Consumer;
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
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                var builder = new KafkaConsumerBuilder<TKey, TValue>()
                    .WithConsumerKey(consumerKey)
                    .WithLoggerFactory(loggerFactory)
                    .WithServiceProvider(serviceProvider);

                configureConsumer.Invoke(serviceProvider, builder);

                return builder;
            });

            services.TryAddKeyedSingleton(consumerKey, (serviceProvider, _) =>
            {
                var builder = serviceProvider.GetRequiredKeyedService<IKafkaConsumerBuilder<TKey, TValue>>(consumerKey);

                var consumer = builder.Build();

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
