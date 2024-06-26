using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal static class ProtobufNetSerializerFactory
    {
        public static ProtobufNetSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions,
            object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<ProtobufNetSerializer<T>>(
                serializerKey ?? ProtobufNetSerializerConstants.ProtobufNetSerializerKey) ??
                CreateSerializer<T>(serviceProvider, configuration, (_, builder) => configureOptions?.Invoke(builder));

            return serializer;
        }

        public static ProtobufNetSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions)
        {
            var options = ProtobufNetSerializerOptionsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureOptions);

            var serializer = new ProtobufNetSerializer<T>(options);

            return serializer;
        }
    }
}
