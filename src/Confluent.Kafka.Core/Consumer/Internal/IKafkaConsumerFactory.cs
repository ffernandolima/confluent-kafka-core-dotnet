using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal interface IKafkaConsumerFactory
    {
        IKafkaConsumer<TKey, TValue> GetOrCreateConsumer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey);

        IKafkaConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey);
    }
}
