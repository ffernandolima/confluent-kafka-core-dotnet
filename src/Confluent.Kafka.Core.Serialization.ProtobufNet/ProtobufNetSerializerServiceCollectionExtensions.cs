using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Confluent.Kafka.Core.Serialization.ProtobufNet.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProtobufNetSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddProtobufNetSerializer<T>(
            this IServiceCollection services,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serializer = ProtobufNetSerializerFactory.CreateSerializer<T>(configureOptions);

            services.TryAddSingleton(serializer);

            return services;
        }
    }
}
