using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Confluent.Kafka.Core.Consumer
{
    public static class KafkaConsumerBuilderExtensions
    {
        public static IKafkaConsumerBuilder<TKey, TValue> WithInterceptorsFromAssemblies<TKey, TValue>(
            this IKafkaConsumerBuilder<TKey, TValue> builder,
            params Assembly[] scanningAssemblies)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var interceptorType = typeof(IKafkaConsumerInterceptor<TKey, TValue>);

            var interceptorTypes = AssemblyScanner.Scan(
                scanningAssemblies,
                scannedType => interceptorType.IsAssignableFrom(scannedType) &&
                    !scannedType.IsInterface &&
                    !scannedType.IsAbstract);

            if (interceptorTypes.Any())
            {
                var interceptors = interceptorTypes
                    .Select(interceptorType => builder.ServiceProvider is null
                        ? Activator.CreateInstance(interceptorType)
                        : ActivatorUtilities.CreateInstance(builder.ServiceProvider, interceptorType))
                    .Cast<IKafkaConsumerInterceptor<TKey, TValue>>()
                    .ToArray();

                builder.WithInterceptors(interceptors);
            }

            return builder;
        }
    }
}
