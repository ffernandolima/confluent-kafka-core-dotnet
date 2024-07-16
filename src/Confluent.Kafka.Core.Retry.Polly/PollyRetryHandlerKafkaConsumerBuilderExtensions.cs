using Confluent.Kafka.Core.Retry.Polly;
using Confluent.Kafka.Core.Retry.Polly.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer
{
    public static class PollyRetryHandlerKafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithPollyRetryHandler<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null,
            object handlerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
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
