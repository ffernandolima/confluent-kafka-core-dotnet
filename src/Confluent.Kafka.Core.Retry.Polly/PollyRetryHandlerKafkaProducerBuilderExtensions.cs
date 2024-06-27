using Confluent.Kafka.Core.Retry.Polly;
using Confluent.Kafka.Core.Retry.Polly.Internal;
using System;

namespace Confluent.Kafka.Core.Producer
{
    public static class PollyRetryHandlerKafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithPollyRetryHandler<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> builder,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null,
            object handlerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var retryHandler = PollyRetryHandlerFactory.Instance.GetOrCreateRetryHandler<TKey, TValue>(
                builder.ServiceProvider,
                builder.Configuration,
                builder.LoggerFactory,
                configureOptions,
                handlerKey);

            builder.WithRetryHandler(retryHandler);

            return builder;
        }
    }
}
