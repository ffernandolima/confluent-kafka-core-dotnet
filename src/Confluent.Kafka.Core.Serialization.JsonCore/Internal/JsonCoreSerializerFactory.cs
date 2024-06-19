using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.JsonCore.Internal
{
    internal static class JsonCoreSerializerFactory
    {
        public static JsonCoreSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IJsonSerializerOptionsBuilder> configureOptions,
            object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<JsonCoreSerializer<T>>(
                serializerKey ?? JsonCoreSerializerConstants.JsonCoreSerializerKey) ??
                CreateSerializer<T>(serviceProvider, (_, builder) => configureOptions?.Invoke(builder));

            return serializer;
        }

        public static JsonCoreSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IJsonSerializerOptionsBuilder> configureOptions)
        {
            var options = JsonSerializerOptionsBuilder.Build(serviceProvider, configureOptions);

            var serializer = new JsonCoreSerializer<T>(options);

            return serializer;
        }
    }
}
