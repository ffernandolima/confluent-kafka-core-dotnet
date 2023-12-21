using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Consumer.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Consumer
{
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
