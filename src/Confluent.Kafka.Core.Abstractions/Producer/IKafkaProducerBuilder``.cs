using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerBuilder<TKey, TValue>
    {
        ILoggerFactory LoggerFactory { get; }

        IServiceProvider ServiceProvider { get; }

        IKafkaProducerConfig ProducerConfig { get; }

        IKafkaProducerBuilder<TKey, TValue> WithStatisticsHandler(Action<IProducer<TKey, TValue>, string> statisticsHandler);

        IKafkaProducerBuilder<TKey, TValue> WithPartitioner(string topic, PartitionerDelegate partitioner);

        IKafkaProducerBuilder<TKey, TValue> WithDefaultPartitioner(PartitionerDelegate partitioner);

        IKafkaProducerBuilder<TKey, TValue> WithErrorHandler(Action<IProducer<TKey, TValue>, Error> errorHandler);

        IKafkaProducerBuilder<TKey, TValue> WithLogHandler(Action<IProducer<TKey, TValue>, LogMessage> logHandler);

        IKafkaProducerBuilder<TKey, TValue> WithOAuthBearerTokenRefreshHandler(Action<IProducer<TKey, TValue>, string> oAuthBearerTokenRefreshHandler);

        IKafkaProducerBuilder<TKey, TValue> WithKeySerializer(ISerializer<TKey> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithKeySerializer(IAsyncSerializer<TKey> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithValueSerializer(ISerializer<TValue> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithValueSerializer(IAsyncSerializer<TValue> serializer);

        IKafkaProducerBuilder<TKey, TValue> WithProducerKey(object producerKey);

        IKafkaProducerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory);

        IKafkaProducerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider);

        IKafkaProducerBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, object> messageIdHandler);

        IKafkaProducerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler);

        IKafkaProducerBuilder<TKey, TValue> WithHandlerFactory(IKafkaProducerHandlerFactory<TKey, TValue> handlerFactory);

        IKafkaProducerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaProducerInterceptor<TKey, TValue>> interceptors);

        IKafkaProducerBuilder<TKey, TValue> WithConfigureProducer(Action<IServiceProvider, IKafkaProducerConfigBuilder> configureProducer);

        IKafkaProducer<TKey, TValue> Build();
    }
}
