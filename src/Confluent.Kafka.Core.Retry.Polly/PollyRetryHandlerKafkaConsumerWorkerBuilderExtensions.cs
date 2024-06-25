﻿using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Retry.Polly.Internal;
using System;

namespace Confluent.Kafka.Core.Retry.Polly
{
    public static class PollyRetryHandlerKafkaConsumerWorkerBuilderExtensions
    {
        public static IKafkaConsumerWorkerBuilder<TKey, TValue> WithPollyRetryHandler<TKey, TValue>(
            this IKafkaConsumerWorkerBuilder<TKey, TValue> builder,
            Action<IPollyRetryHandlerOptionsBuilder> configureOptions = null,
            object handlerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var retryHandler = PollyRetryHandlerFactory.GetOrCreateRetryHandler<TKey, TValue>(
                builder.ServiceProvider,
                builder.LoggerFactory,
                configureOptions,
                handlerKey);

            builder.WithRetryHandler(retryHandler);

            return builder;
        }
    }
}