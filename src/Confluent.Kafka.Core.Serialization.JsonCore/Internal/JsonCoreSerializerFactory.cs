using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.JsonCore.Internal
{
    internal sealed class JsonCoreSerializerFactory : IJsonCoreSerializerFactory
    {
        private static readonly Lazy<JsonCoreSerializerFactory> Factory = new(
           () => new JsonCoreSerializerFactory(), isThreadSafe: true);

        public static JsonCoreSerializerFactory Instance => Factory.Value;

        private JsonCoreSerializerFactory()
        { }

        public JsonCoreSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IJsonSerializerOptionsBuilder> configureOptions,
            object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<JsonCoreSerializer<T>>(
                serializerKey ?? JsonCoreSerializerConstants.JsonCoreSerializerKey) ??
                CreateSerializer<T>(serviceProvider, configuration, (_, builder) => configureOptions?.Invoke(builder));

            return serializer;
        }

        public JsonCoreSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IJsonSerializerOptionsBuilder> configureOptions)
        {
            var options = JsonSerializerOptionsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureOptions);

            var serializer = new JsonCoreSerializer<T>(options);

            return serializer;
        }
    }
}
