using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal static class ProtobufNetSerializerFactory
    {
        public static ProtobufNetSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            var serializer = serviceProvider?.GetService<ProtobufNetSerializer<T>>() ?? 
                CreateSerializer<T>(configureOptions);

            return serializer;
        }

        public static ProtobufNetSerializer<T> CreateSerializer<T>(
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null)
        {
            var options = ProtobufNetSerializerOptionsBuilder.Build(configureOptions);

            var serializer = new ProtobufNetSerializer<T>(options);

            return serializer;
        }
    }
}
