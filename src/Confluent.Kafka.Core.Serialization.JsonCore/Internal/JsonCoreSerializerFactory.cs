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
                CreateSerializer<T>(configureOptions);

            return serializer;
        }

        public static JsonCoreSerializer<T> CreateSerializer<T>(
            Action<IJsonSerializerOptionsBuilder> configureOptions)
        {
            var settings = JsonSerializerOptionsBuilder.Build(configureOptions);

            var serializer = new JsonCoreSerializer<T>(settings);

            return serializer;
        }
    }
}
