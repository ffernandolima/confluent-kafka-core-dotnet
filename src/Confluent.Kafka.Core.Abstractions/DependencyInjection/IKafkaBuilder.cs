using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Hosting.Retry;
using Confluent.Kafka.Core.Producer;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public interface IKafkaBuilder
    {
        IServiceCollection Services { get; }

        IKafkaBuilder AddKafkaConsumer<TKey, TValue>(
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey = null);

        IKafkaBuilder AddKafkaConsumerHandlerFactory<TKey, TValue>(
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions = null,
            object consumerKey = null);

        IKafkaBuilder AddKafkaConsumerHandlerFactory<TKey, TValue>(
            Func<IServiceProvider, IKafkaConsumerHandlerFactory<TKey, TValue>> implementationFactory,
            object consumerKey = null);

        IKafkaBuilder AddKafkaConsumerWorker<TKey, TValue>(
            Action<IServiceProvider, IKafkaConsumerWorkerBuilder<TKey, TValue>> configureWorker,
            object workerKey = null);

        IKafkaBuilder AddKafkaRetryConsumerWorker(
            Action<IServiceProvider, IKafkaRetryConsumerWorkerBuilder> configureWorker,
            object workerKey = null);

        IKafkaBuilder AddKafkaProducer<TKey, TValue>(
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey = null);

        IKafkaBuilder AddKafkaProducerHandlerFactory<TKey, TValue>(
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions = null,
            object producerKey = null);

        IKafkaBuilder AddKafkaProducerHandlerFactory<TKey, TValue>(
            Func<IServiceProvider, IKafkaProducerHandlerFactory<TKey, TValue>> implementationFactory,
            object producerKey = null);

        IKafkaBuilder AddKafkaDiagnostics(
            Action<IServiceProvider, IKafkaEnrichmentOptionsBuilder> configureOptions = null);
    }
}
