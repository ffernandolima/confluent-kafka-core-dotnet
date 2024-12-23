﻿using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerBuilder<TKey, TValue>
    {
        IConfiguration Configuration { get; }

        ILoggerFactory LoggerFactory { get; }

        IServiceProvider ServiceProvider { get; }

        IKafkaProducerConfig ProducerConfig { get; }

        IKafkaProducerBuilder<TKey, TValue> WithStatisticsHandler(Action<IProducer<TKey, TValue>, string> statisticsHandler);

        IKafkaProducerBuilder<TKey, TValue> WithPartitioner(string topic, PartitionerDelegate partitioner);

        IKafkaProducerBuilder<TKey, TValue> WithDefaultPartitioner(PartitionerDelegate partitioner);

        IKafkaProducerBuilder<TKey, TValue> WithErrorHandler(Action<IProducer<TKey, TValue>, Error> errorHandler);

        IKafkaProducerBuilder<TKey, TValue> WithLogHandler(Action<IProducer<TKey, TValue>, LogMessage> logHandler);

        IKafkaProducerBuilder<TKey, TValue> WithOAuthBearerTokenRefreshHandler(Action<IClient, string> oAuthBearerTokenRefreshHandler);

        IKafkaProducerBuilder<TKey, TValue> WithKeySerializer(ISerializer<TKey> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithKeySerializer(IAsyncSerializer<TKey> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithValueSerializer(ISerializer<TValue> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithValueSerializer(IAsyncSerializer<TValue> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithProducerKey(object producerKey);

        IKafkaProducerBuilder<TKey, TValue> WithConfiguration(IConfiguration configuration);

        IKafkaProducerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory);

        IKafkaProducerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider);

        IKafkaProducerBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, object> messageIdHandler);

        IKafkaProducerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler);

        IKafkaProducerBuilder<TKey, TValue> WithHandlerFactory(IKafkaProducerHandlerFactory<TKey, TValue> handlerFactory);

        IKafkaProducerBuilder<TKey, TValue> WithInterceptor(IKafkaProducerInterceptor<TKey, TValue> interceptor);

        IKafkaProducerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> interceptors);

        IKafkaProducerBuilder<TKey, TValue> WithProducerConfiguration(Action<IKafkaProducerConfigBuilder> configureProducer);

#if NETSTANDARD2_0_OR_GREATER
        IProducer<TKey, TValue> Build();
#else
        IKafkaProducer<TKey, TValue> Build();
#endif
    }
}
