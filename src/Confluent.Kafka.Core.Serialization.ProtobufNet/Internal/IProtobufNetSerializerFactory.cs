using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal interface IProtobufNetSerializerFactory
    {
        ProtobufNetSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions,
            object serializerKey);

        ProtobufNetSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions);
    }
}
