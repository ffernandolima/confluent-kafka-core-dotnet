using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using System;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public static class KafkaRetryConsumerWorkerBuilderExtensions
    {
        public static IKafkaRetryConsumerWorkerBuilder WithConsumer(
            this IKafkaRetryConsumerWorkerBuilder workerBuilder,
            Action<IKafkaConsumerBuilder<byte[], KafkaMetadataMessage>> configureConsumer = null,
            object consumerKey = null)
        {
            if (workerBuilder is null)
            {
                throw new ArgumentNullException(nameof(workerBuilder));
            }

            var consumer = KafkaConsumerFactory.Instance.GetOrCreateConsumer<byte[], KafkaMetadataMessage>(
                workerBuilder.ServiceProvider,
                workerBuilder.Configuration,
                workerBuilder.LoggerFactory,
                builder =>
                {
                    configureConsumer?.Invoke(builder);

                    try
                    {
                        builder.WithConsumerConfiguration(builder =>
                        {
                            builder.WithEnableAutoCommit(false);
                            builder.WithCommitAfterConsuming(false);
                            builder.WithEnableAutoOffsetStore(false);
                        });
                    }
                    catch (Exception ex)
                    {
                        if (ex is not InvalidOperationException ioex || ioex.Message != "Consumer may not be configured more than once.")
                        {
                            throw;
                        }
                    }
                },
                consumerKey);

            workerBuilder.WithConsumer(consumer);

            return workerBuilder;
        }

        public static IKafkaRetryConsumerWorkerBuilder WithSourceProducer(
            this IKafkaRetryConsumerWorkerBuilder workerBuilder,
            Action<IKafkaProducerBuilder<byte[], byte[]>> configureProducer = null,
            object producerKey = null)
        {
            if (workerBuilder is null)
            {
                throw new ArgumentNullException(nameof(workerBuilder));
            }

            var retryProducer = KafkaProducerFactory.Instance.GetOrCreateProducer(
                workerBuilder.ServiceProvider,
                workerBuilder.Configuration,
                workerBuilder.LoggerFactory,
                configureProducer,
                producerKey);

            workerBuilder.WithSourceProducer(retryProducer);

            return workerBuilder;
        }

        public static IKafkaRetryConsumerWorkerBuilder WithDeadLetterProducer(
            this IKafkaRetryConsumerWorkerBuilder workerBuilder,
            Action<IKafkaProducerBuilder<byte[], KafkaMetadataMessage>> configureProducer = null,
            object producerKey = null)
        {
            if (workerBuilder is null)
            {
                throw new ArgumentNullException(nameof(workerBuilder));
            }

            var deadLetterProducer = KafkaProducerFactory.Instance.GetOrCreateProducer(
                workerBuilder.ServiceProvider,
                workerBuilder.Configuration,
                workerBuilder.LoggerFactory,
                configureProducer,
                producerKey);

            workerBuilder.WithDeadLetterProducer(deadLetterProducer);

            return workerBuilder;
        }
    }
}
