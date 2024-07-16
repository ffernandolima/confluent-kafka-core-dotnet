using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Producer
{
    public static partial class KafkaProducerBuilderExtensions
    {
        public static IKafkaProducerBuilder<TKey, TValue> WithHandlerFactory<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> producerBuilder,
            Action<IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions = null,
            object producerKey = null)
        {
            if (producerBuilder is null)
            {
                throw new ArgumentNullException(nameof(producerBuilder));
            }

            var handlerFactory = KafkaProducerHandlerFactory.Instance.GetOrCreateHandlerFactory<TKey, TValue>(
                producerBuilder.ServiceProvider,
                producerBuilder.Configuration,
                producerBuilder.LoggerFactory,
                configureOptions,
                producerKey);

            producerBuilder.WithHandlerFactory(handlerFactory);

            return producerBuilder;
        }

        public static IKafkaProducerBuilder<TKey, TValue> WithInterceptorsFromAssemblies<TKey, TValue>(
            this IKafkaProducerBuilder<TKey, TValue> producerBuilder,
            params Assembly[] assemblies)
        {
            if (producerBuilder is null)
            {
                throw new ArgumentNullException(nameof(producerBuilder));
            }

            var interceptorType = typeof(IKafkaProducerInterceptor<TKey, TValue>);

            var interceptorTypes = AssemblyScanner.Scan(
                assemblies,
                loadedType => interceptorType.IsAssignableFrom(loadedType) &&
                    !loadedType.IsInterface &&
                    !loadedType.IsAbstract);

            if (interceptorTypes.Length > 0)
            {
                var interceptors = interceptorTypes
                    .Select(interceptorType => ObjectFactory.Instance.TryCreateInstance(producerBuilder.ServiceProvider, interceptorType))
                    .Where(interceptor => interceptor is not null)
                    .Cast<IKafkaProducerInterceptor<TKey, TValue>>()
                    .ToArray();

                producerBuilder.WithInterceptors(interceptors);
            }

            return producerBuilder;
        }
    }
}
