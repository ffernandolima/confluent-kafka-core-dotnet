using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Retry.Polly;
using Confluent.Kafka.Core.Retry.Polly.Internal;
using System;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public static class PollyRetryHandlerKafkaRetryConsumerWorkerBuilderExtensions
    {
        public static IKafkaRetryConsumerWorkerBuilder WithPollyRetryHandler(
            this IKafkaRetryConsumerWorkerBuilder builder,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null,
            object handlerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var retryHandler = PollyRetryHandlerFactory.Instance.GetOrCreateRetryHandler<byte[], KafkaMetadataMessage>(
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
