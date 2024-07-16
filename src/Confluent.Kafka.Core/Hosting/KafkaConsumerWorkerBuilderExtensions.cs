using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Hosting
{
    public static partial class KafkaConsumerWorkerBuilderExtensions
    {
        public static IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumer<TKey, TValue>(
            this IKafkaConsumerWorkerBuilder<TKey, TValue> workerBuilder,
            Action<IKafkaConsumerBuilder<TKey, TValue>> configureConsumer = null,
            object consumerKey = null)
        {
            if (workerBuilder is null)
            {
                throw new ArgumentNullException(nameof(workerBuilder));
            }

            var consumer = KafkaConsumerFactory.Instance.GetOrCreateConsumer<TKey, TValue>(
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

        public static IKafkaConsumerWorkerBuilder<TKey, TValue> WithRetryProducer<TKey, TValue>(
            this IKafkaConsumerWorkerBuilder<TKey, TValue> workerBuilder,
            Action<IKafkaProducerBuilder<byte[], KafkaMetadataMessage>> configureProducer = null,
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

            workerBuilder.WithRetryProducer(retryProducer);

            return workerBuilder;
        }

        public static IKafkaConsumerWorkerBuilder<TKey, TValue> WithDeadLetterProducer<TKey, TValue>(
            this IKafkaConsumerWorkerBuilder<TKey, TValue> workerBuilder,
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

        public static IKafkaConsumerWorkerBuilder<TKey, TValue> WithConsumeResultHandlersFromAssemblies<TKey, TValue>(
            this IKafkaConsumerWorkerBuilder<TKey, TValue> workerBuilder,
            params Assembly[] assemblies)
        {
            if (workerBuilder is null)
            {
                throw new ArgumentNullException(nameof(workerBuilder));
            }

            var consumeResultHandlerType = typeof(IConsumeResultHandler<TKey, TValue>);

            var consumeResultHandlerTypes = AssemblyScanner.Scan(
                assemblies,
                loadedType => consumeResultHandlerType.IsAssignableFrom(loadedType) &&
                    !loadedType.IsInterface &&
                    !loadedType.IsAbstract);

            if (consumeResultHandlerTypes.Length > 0)
            {
                var consumeResultHandlers = consumeResultHandlerTypes
                    .Select(consumeResultHandlerType =>
                        ObjectFactory.Instance.TryCreateInstance(workerBuilder.ServiceProvider, consumeResultHandlerType))
                    .Where(consumeResultHandler => consumeResultHandler is not null)
                    .Cast<IConsumeResultHandler<TKey, TValue>>()
                    .ToArray();

                workerBuilder.WithConsumeResultHandlers(consumeResultHandlers);
            }

            return workerBuilder;
        }
    }
}
