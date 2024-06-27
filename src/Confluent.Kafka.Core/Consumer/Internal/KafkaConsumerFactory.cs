using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerFactory
    {
        private static readonly Lazy<KafkaConsumerFactory> Factory = new(
            () => new KafkaConsumerFactory(), isThreadSafe: true);

        public static KafkaConsumerFactory Instance => Factory.Value;

        private KafkaConsumerFactory()
        { }

        public IKafkaConsumer<TKey, TValue> GetOrCreateConsumer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey)
        {
            var consumer = serviceProvider?.GetKeyedService<IKafkaConsumer<TKey, TValue>>(consumerKey) ??
                CreateConsumer<TKey, TValue>(
                    serviceProvider,
                    configuration,
                    loggerFactory,
                    (_, builder) => configureConsumer?.Invoke(builder),
                    consumerKey);

            return consumer;
        }
        public IKafkaConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            Action<IServiceProvider, IKafkaConsumerBuilder<TKey, TValue>> configureConsumer,
            object consumerKey)
        {
            var consumer = KafkaConsumerBuilder<TKey, TValue>.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                loggerFactory ?? serviceProvider?.GetService<ILoggerFactory>(),
                configureConsumer,
                consumerKey);

            return consumer;
        }
    }
}
