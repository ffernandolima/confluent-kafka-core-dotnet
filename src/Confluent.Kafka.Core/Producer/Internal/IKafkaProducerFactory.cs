using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal interface IKafkaProducerFactory
    {
        IKafkaProducer<TKey, TValue> GetOrCreateProducer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey);

        IKafkaProducer<TKey, TValue> CreateProducer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaProducerBuilder<TKey, TValue>> configureProducer,
            object producerKey);
    }
}
