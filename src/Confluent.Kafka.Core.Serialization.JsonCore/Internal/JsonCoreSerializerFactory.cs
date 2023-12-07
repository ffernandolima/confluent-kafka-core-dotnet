using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.JsonCore.Internal
{
    internal static class JsonCoreSerializerFactory
    {
        public static JsonCoreSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            var serializer = serviceProvider?.GetService<JsonCoreSerializer<T>>() ?? 
                CreateSerializer<T>(configureOptions);

            return serializer;
        }

        public static JsonCoreSerializer<T> CreateSerializer<T>(
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            var settings = JsonSerializerOptionsBuilder.Build(configureOptions);

            var serializer = new JsonCoreSerializer<T>(settings);

            return serializer;
        }
    }
}
