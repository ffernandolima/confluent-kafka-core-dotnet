using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal static class ProtobufNetSerializerFactory
    {
        public static ProtobufNetSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions,
            object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<ProtobufNetSerializer<T>>(
                serializerKey ?? ProtobufNetSerializerConstants.ProtobufNetSerializerKey) ??
                CreateSerializer<T>(serviceProvider, (_, builder) => configureOptions?.Invoke(builder));

            return serializer;
        }

        public static ProtobufNetSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions)
        {
            var options = ProtobufNetSerializerOptionsBuilder.Build(serviceProvider, configureOptions);

            var serializer = new ProtobufNetSerializer<T>(options);

            return serializer;
        }
    }
}
