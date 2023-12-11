using Confluent.Kafka.Core.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Consumer
{
    public static class KafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithInterceptorsFromAssemblies<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            params Assembly[] assemblies)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
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
                    .Select(interceptorType => ObjectFactory.TryCreateInstance(builder.ServiceProvider, interceptorType))
                    .Where(interceptor => interceptor is not null)
                    .Cast<IKafkaConsumerInterceptor<TKey, TValue>>()
                    .ToArray();

                builder.WithInterceptors(interceptors);
            }

            return builder;
        }
    }
}
