using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Retry;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerBuilder<TKey, TValue>
    {
        ILoggerFactory LoggerFactory { get; }

        IServiceProvider ServiceProvider { get; }

        IKafkaConsumerConfig ConsumerConfig { get; }

        IKafkaConsumerBuilder<TKey, TValue> WithStatisticsHandler(Action<IConsumer<TKey, TValue>, string> statisticsHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithErrorHandler(Action<IConsumer<TKey, TValue>, Error> errorHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithLogHandler(Action<IConsumer<TKey, TValue>, LogMessage> logHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithOAuthBearerTokenRefreshHandler(Action<IConsumer<TKey, TValue>, string> oAuthBearerTokenRefreshHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithKeyDeserializer(IDeserializer<TKey> deserializer);

        IKafkaConsumerBuilder<TKey, TValue> WithKeyDeserializer(IAsyncDeserializer<TKey> deserializer);

        IKafkaConsumerBuilder<TKey, TValue> WithValueDeserializer(IDeserializer<TValue> deserializer);

        IKafkaConsumerBuilder<TKey, TValue> WithValueDeserializer(IAsyncDeserializer<TValue> deserializer);

        IKafkaConsumerBuilder<TKey, TValue> WithPartitionsAssignedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartition>, IEnumerable<TopicPartitionOffset>> partitionsAssignedHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithPartitionsAssignedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartition>> partitionAssignmentHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithPartitionsRevokedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsRevokedHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithPartitionsRevokedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsRevokedHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithPartitionsLostHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsLostHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithPartitionsLostHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsLostHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithOffsetsCommittedHandler(Action<IConsumer<TKey, TValue>, CommittedOffsets> offsetsCommittedHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithConsumerType(Type consumerType);

        IKafkaConsumerBuilder<TKey, TValue> WithConsumerKey(object consumerKey);

        IKafkaConsumerBuilder<TKey, TValue> WithLoggerFactory(ILoggerFactory loggerFactory);

        IKafkaConsumerBuilder<TKey, TValue> WithServiceProvider(IServiceProvider serviceProvider);

        IKafkaConsumerBuilder<TKey, TValue> WithMessageIdHandler(Func<TValue, object> messageIdHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithRetryHandler(IRetryHandler<TKey, TValue> retryHandler);

        IKafkaConsumerBuilder<TKey, TValue> WithHandlerFactory(IKafkaConsumerHandlerFactory<TKey, TValue> handlerFactory);

        IKafkaConsumerBuilder<TKey, TValue> WithDeadLetterProducer(IKafkaProducer<byte[], KafkaMetadataMessage> deadLetterProducer);

        IKafkaConsumerBuilder<TKey, TValue> WithInterceptors(IEnumerable<IKafkaConsumerInterceptor<TKey, TValue>> interceptors);

        IKafkaConsumerBuilder<TKey, TValue> WithConfigureConsumer(Action<IServiceProvider, IKafkaConsumerConfigBuilder> configureConsumer);

        IKafkaConsumer<TKey, TValue> Build();
    }
}
