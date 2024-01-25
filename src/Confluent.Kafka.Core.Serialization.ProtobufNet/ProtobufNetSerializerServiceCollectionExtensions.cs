using Confluent.Kafka;
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
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serializer = ProtobufNetSerializerFactory.CreateSerializer<T>(configureOptions);
            var serviceKey = serializerKey ?? ProtobufNetSerializerConstants.ProtobufNetSerializerKey;

            services.TryAddKeyedSingleton(serviceKey, serializer);
            services.TryAddKeyedSingleton<ISerializer<T>>(serviceKey, serializer);
            services.TryAddKeyedSingleton<IDeserializer<T>>(serviceKey, serializer);

            return services;
        }
    }
}
