using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Consumer
{
    using Confluent.Kafka.Core.Internal;

    public static class KafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithHandlerFactory<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions = null,
            object consumerKey = null)
        {
            if (consumerBuilder is null)
            {
                throw new ArgumentNullException(nameof(consumerBuilder), $"{nameof(consumerBuilder)} cannot be null.");
            }

            var handlerFactory = KafkaConsumerHandlerFactory.GetOrCreateHandlerFactory<TKey, TValue>(
                consumerBuilder.ServiceProvider,
                consumerBuilder.LoggerFactory,
                configureOptions,
                consumerKey);

            consumerBuilder.WithHandlerFactory(handlerFactory);

            return consumerBuilder;
        }

        public static IKafkaConsumerBuilder<TKey, TValue> WithDeadLetterProducer<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> consumerBuilder,
            Action<IServiceProvider, IKafkaProducerBuilder<byte[], KafkaMetadataMessage>> configureProducer = null,
            object producerKey = null)
        {
            if (consumerBuilder is null)
            {
                throw new ArgumentNullException(nameof(consumerBuilder), $"{nameof(consumerBuilder)} cannot be null.");
            }

            var producerbuilder = consumerBuilder.ServiceProvider?.GetKeyedService<IKafkaProducerBuilder<byte[], KafkaMetadataMessage>>(producerKey) ??
                new KafkaProducerBuilder<byte[], KafkaMetadataMessage>()
                    .WithProducerKey(producerKey)
                    .WithLoggerFactory(
                        consumerBuilder.LoggerFactory ??
                        consumerBuilder.ServiceProvider?.GetService<ILoggerFactory>())
                     .WithServiceProvider(consumerBuilder.ServiceProvider);

            configureProducer?.Invoke(consumerBuilder.ServiceProvider, producerbuilder);

            consumerBuilder.WithDeadLetterProducer(producerbuilder.Build());

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
