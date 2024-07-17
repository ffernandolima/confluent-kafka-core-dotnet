using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerFactory : IKafkaProducerFactory
    {
        private static readonly Lazy<KafkaProducerFactory> Factory = new(
            () => new KafkaProducerFactory(), isThreadSafe: true);

        public static KafkaProducerFactory Instance => Factory.Value;

        private KafkaProducerFactory()
        { }

        public IKafkaProducer<TKey, TValue> GetOrCreateProducer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey)
        {
            var producer = serviceProvider?.GetKeyedService<IKafkaProducer<TKey, TValue>>(producerKey) ??
                CreateProducer<TKey, TValue>(
                    serviceProvider,
                    configuration,
                    loggerFactory,
                    (_, builder) => configureProducer?.Invoke(builder),
                    producerKey);

            return producer;
        }

        public IKafkaProducer<TKey, TValue> CreateProducer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey)
        {
            var producer = KafkaProducerBuilder<TKey, TValue>.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                configureProducer,
                producerKey);

            return producer;
        }
    }
}
