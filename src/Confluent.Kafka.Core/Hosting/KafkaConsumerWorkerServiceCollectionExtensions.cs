using Confluent.Kafka.Core.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaConsumerWorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumerWorker<TKey, TValue>(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaConsumerWorkerBuilder<TKey, TValue>> configureWorker,
            object workerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureWorker is null)
            {
                throw new ArgumentNullException(nameof(configureWorker));
            }

            services.AddKafkaDiagnostics();

            services.TryAddKeyedSingleton(workerKey, (serviceProvider, _) =>
            {
                var builder = KafkaConsumerWorkerBuilder<TKey, TValue>.Configure(
                    serviceProvider,
                    serviceProvider.GetService<IConfiguration>(),
                    serviceProvider.GetService<ILoggerFactory>(),
                    configureWorker,
                    workerKey);

                return builder;
            });

            services.TryAddKeyedSingleton(workerKey, (serviceProvider, _) =>
            {
                var builder = serviceProvider.GetRequiredKeyedService<IKafkaConsumerWorkerBuilder<TKey, TValue>>(workerKey);

                var consumerWorker = builder.Build();

                return consumerWorker;
            });

            if (workerKey is null)
            {
                services.TryAddEnumerable(
                    ServiceDescriptor.Singleton<IHostedService, KafkaConsumerService<TKey, TValue>>(
                        serviceProvider =>
                        {
                            var consumerWorker = serviceProvider.GetRequiredService<IKafkaConsumerWorker<TKey, TValue>>();

                            var consumerService = new KafkaConsumerService<TKey, TValue>(consumerWorker);

                            return consumerService;
                        }));
            }
            else
            {
                services.TryAddEnumerable(
                    ServiceDescriptor.KeyedSingleton<IHostedService, KafkaConsumerService<TKey, TValue>>(
                        workerKey,
                        (serviceProvider, _) =>
                        {
                            var consumerWorker = serviceProvider.GetRequiredKeyedService<IKafkaConsumerWorker<TKey, TValue>>(workerKey);

                            var consumerService = new KafkaConsumerService<TKey, TValue>(consumerWorker);

                            return consumerService;
                        }));
            }

            return services;
        }
    }
}
