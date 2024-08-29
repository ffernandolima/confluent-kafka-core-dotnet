using Confluent.Kafka.Core.Consumer;
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
                throw new ArgumentNullException(nameof(services));
            }

            if (configureConsumer is null)
            {
                throw new ArgumentNullException(nameof(configureConsumer));
            }

            services.TryAddKeyedSingleton(consumerKey, (serviceProvider, _) =>
            {
                var builder = KafkaConsumerBuilder<TKey, TValue>.Configure(
                    serviceProvider,
                    serviceProvider.GetService<IConfiguration>(),
                    serviceProvider.GetService<ILoggerFactory>(),
                    configureConsumer,
                    consumerKey);

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

            return services;
        }
    }
}
