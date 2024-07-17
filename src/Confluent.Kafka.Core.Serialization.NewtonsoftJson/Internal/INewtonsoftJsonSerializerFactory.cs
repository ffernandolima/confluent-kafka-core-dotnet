using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal
{
    internal interface INewtonsoftJsonSerializerFactory
    {
        NewtonsoftJsonSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IJsonSerializerSettingsBuilder> configureSettings,
            object serializerKey);

        NewtonsoftJsonSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IJsonSerializerSettingsBuilder> configureSettings);
    }
}
