using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Hosting.Retry;
using Confluent.Kafka.Core.Producer;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection.Internal
{
    internal sealed class KafkaBuilder : IKafkaBuilder
    {
        public IServiceCollection Services { get; }

        public KafkaBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IKafkaBuilder AddKafkaConsumer<TKey, TValue>(
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey = null)
        {
            if (configureConsumer is null)
            {
                throw new ArgumentNullException(nameof(configureConsumer));
            }

            Services.AddKafkaConsumer(configureConsumer, consumerKey);

            return this;
        }

        public IKafkaBuilder AddKafkaConsumerHandlerFactory<TKey, TValue>(
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions = null,
            object consumerKey = null)
        {
            Services.AddKafkaConsumerHandlerFactory<TKey, TValue>(configureOptions, consumerKey);

            return this;
        }

        public IKafkaBuilder AddKafkaConsumerHandlerFactory<TKey, TValue>(
            Func<IServiceProvider, IKafkaConsumerHandlerFactory<TKey, TValue>> implementationFactory,
            object consumerKey = null)
        {
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }

            Services.AddKafkaConsumerHandlerFactory(implementationFactory, consumerKey);

            return this;
        }

        public IKafkaBuilder AddKafkaConsumerWorker<TKey, TValue>(
            Action<IServiceProvider, IKafkaConsumerWorkerBuilder<TKey, TValue>> configureWorker,
            object workerKey = null)
        {
            if (configureWorker is null)
            {
                throw new ArgumentNullException(nameof(configureWorker));
            }

            Services.AddKafkaConsumerWorker(configureWorker, workerKey);

            return this;
        }

        public IKafkaBuilder AddKafkaRetryConsumerWorker(
            Action<IServiceProvider, IKafkaRetryConsumerWorkerBuilder> configureWorker,
            object workerKey = null)
        {
            if (configureWorker is null)
            {
                throw new ArgumentNullException(nameof(configureWorker));
            }

            Services.AddKafkaRetryConsumerWorker(configureWorker, workerKey);

            return this;
        }

        public IKafkaBuilder AddKafkaProducer<TKey, TValue>(
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey = null)
        {
            if (configureProducer is null)
            {
                throw new ArgumentNullException(nameof(configureProducer));
            }

            Services.AddKafkaProducer(configureProducer, configureProducer);

            return this;
        }

        public IKafkaBuilder AddKafkaProducerHandlerFactory<TKey, TValue>(
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions = null,
            object producerKey = null)
        {
            Services.AddKafkaProducerHandlerFactory<TKey, TValue>(configureOptions, producerKey);

            return this;
        }

        public IKafkaBuilder AddKafkaProducerHandlerFactory<TKey, TValue>(
            Func<IServiceProvider, IKafkaProducerHandlerFactory<TKey, TValue>> implementationFactory,
            object producerKey = null)
        {
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }

            Services.AddKafkaProducerHandlerFactory(implementationFactory, producerKey);

            return this;
        }

        public IKafkaBuilder AddKafkaDiagnostics(
            Action<IServiceProvider, IKafkaEnrichmentOptionsBuilder> configureOptions = null)
        {
            Services.AddKafkaDiagnostics(configureOptions);

            return this;
        }
    }
}
