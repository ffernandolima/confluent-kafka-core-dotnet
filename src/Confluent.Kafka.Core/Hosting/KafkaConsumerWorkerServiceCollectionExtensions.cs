using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Hosting.Internal;
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
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureWorker is null)
            {
                throw new ArgumentNullException(nameof(configureWorker), $"{nameof(configureWorker)} cannot be null.");
            }

            services.TryAddKeyedSingleton(workerKey, (serviceProvider, _) =>
            {
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                var builder = new KafkaConsumerWorkerBuilder<TKey, TValue>()
                    .WithWorkerKey(workerKey)
                    .WithLoggerFactory(loggerFactory)
                    .WithServiceProvider(serviceProvider);

                configureWorker.Invoke(serviceProvider, builder);

                var worker = builder.Build();

                return worker;
            });

            if (workerKey is null)
            {
                services.TryAddEnumerable(
                    ServiceDescriptor.Singleton<IHostedService, IKafkaConsumerWorker<TKey, TValue>>(
                        serviceProvider =>
                        {
                            var worker = serviceProvider.GetRequiredKeyedService<IKafkaConsumerWorker<TKey, TValue>>(workerKey);

                            return worker;
                        }));
            }
            else
            {
                services.TryAddEnumerable(
                    ServiceDescriptor.KeyedSingleton<IHostedService, IKafkaConsumerWorker<TKey, TValue>>(
                        workerKey,
                        (serviceProvider, _) =>
                        {
                            var worker = serviceProvider.GetRequiredKeyedService<IKafkaConsumerWorker<TKey, TValue>>(workerKey);

                            return worker;
                        }));
            }

            return services;
        }
    }
}
