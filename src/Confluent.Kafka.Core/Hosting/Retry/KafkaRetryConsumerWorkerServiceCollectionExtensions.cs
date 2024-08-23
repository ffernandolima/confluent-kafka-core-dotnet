using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public static class KafkaRetryConsumerWorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaRetryConsumerWorker(
            this IServiceCollection services,
            Action<IServiceProvider, IKafkaRetryConsumerWorkerBuilder> configureWorker,
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
                var builder = KafkaRetryConsumerWorkerBuilder.Configure(
                    serviceProvider,
                    serviceProvider.GetService<IConfiguration>(),
                    serviceProvider.GetService<ILoggerFactory>(),
                    configureWorker,
                    workerKey);

                return builder;
            });

            services.TryAddKeyedSingleton(workerKey, (serviceProvider, _) =>
            {
                var builder = serviceProvider.GetRequiredKeyedService<IKafkaRetryConsumerWorkerBuilder>(workerKey);

                var consumerWorker = builder.Build();

                return consumerWorker;
            });

            if (workerKey is null)
            {
                services.TryAddEnumerable(
                    ServiceDescriptor.Singleton<IHostedService, KafkaRetryConsumerService>(
                        serviceProvider =>
                        {
                            var consumerWorker = serviceProvider.GetRequiredService<IKafkaRetryConsumerWorker>();

                            var consumerService = new KafkaRetryConsumerService(consumerWorker);

                            return consumerService;
                        }));
            }
            else
            {
                services.TryAddEnumerable(
                    ServiceDescriptor.KeyedSingleton<IHostedService, KafkaRetryConsumerService>(
                        workerKey,
                        (serviceProvider, _) =>
                        {
                            var consumerWorker = serviceProvider.GetRequiredKeyedService<IKafkaRetryConsumerWorker>(workerKey);

                            var consumerService = new KafkaRetryConsumerService(consumerWorker);

                            return consumerService;
                        }));
            }

            return services;
        }
    }
}
