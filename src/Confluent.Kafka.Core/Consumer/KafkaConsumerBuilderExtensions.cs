using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Confluent.Kafka.Core.Producer.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Consumer
{
    using Confluent.Kafka.Core.Internal;

    public static partial class KafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithHandlerFactory<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder,
            Action<IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions = null,
            object consumerKey = null)
        {
            if (consumerBuilder is null)
            {
                throw new ArgumentNullException(nameof(consumerBuilder), $"{nameof(consumerBuilder)} cannot be null.");
            }

            var handlerFactory = KafkaConsumerHandlerFactory.GetOrCreateHandlerFactory<TKey, TValue>(
                consumerBuilder.ServiceProvider,
                consumerBuilder.Configuration,
                consumerBuilder.LoggerFactory,
                configureOptions,
                consumerKey);

            consumerBuilder.WithHandlerFactory(handlerFactory);

            return consumerBuilder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithDeadLetterProducer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder,
            Action<IKafkaProducerBuilder<byte[], KafkaMetadataMessage>> configureProducer = null,
            object producerKey = null)
        {
            if (consumerBuilder is null)
            {
                throw new ArgumentNullException(nameof(consumerBuilder), $"{nameof(consumerBuilder)} cannot be null.");
            }

            var deadLetterProducer = KafkaProducerFactory.GetOrCreateProducer(
                consumerBuilder.ServiceProvider,
                consumerBuilder.Configuration,
                consumerBuilder.LoggerFactory,
                configureProducer,
                producerKey);

            consumerBuilder.WithDeadLetterProducer(deadLetterProducer);

            return consumerBuilder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithInterceptorsFromAssemblies<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder,
            params Assembly[] assemblies)
        {
            if (consumerBuilder is null)
            {
                throw new ArgumentNullException(nameof(consumerBuilder), $"{nameof(consumerBuilder)} cannot be null.");
            }

            var interceptorType = typeof(IKafkaConsumerInterceptor<TKey, TValue>);

            var interceptorTypes = AssemblyScanner.Scan(
                assemblies,
                loadedType => interceptorType.IsAssignableFrom(loadedType) &&
                    !loadedType.IsInterface &&
                    !loadedType.IsAbstract);

            if (interceptorTypes.Length > 0)
            {
                var interceptors = interceptorTypes
                    .Select(interceptorType => ObjectFactory.TryCreateInstance(consumerBuilder.ServiceProvider, interceptorType))
                    .Where(interceptor => interceptor is not null)
                    .Cast<IKafkaConsumerInterceptor<TKey, TValue>>()
                    .ToArray();

                consumerBuilder.WithInterceptors(interceptors);
            }

            return consumerBuilder;
        }
    }
}
