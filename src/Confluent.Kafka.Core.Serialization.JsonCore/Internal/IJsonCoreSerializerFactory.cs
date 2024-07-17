using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.JsonCore.Internal
{
    internal interface IJsonCoreSerializerFactory
    {
        JsonCoreSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IJsonSerializerOptionsBuilder> configureOptions,
            object serializerKey);

        JsonCoreSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IJsonSerializerOptionsBuilder> configureOptions);
    }
}
