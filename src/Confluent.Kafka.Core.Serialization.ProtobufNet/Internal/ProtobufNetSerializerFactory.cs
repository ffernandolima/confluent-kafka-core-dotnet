﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal static class ProtobufNetSerializerFactory
    {
        public static ProtobufNetSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            var serializer = serviceProvider?.GetKeyedService<ProtobufNetSerializer<T>>(
                serializerKey ?? ProtobufNetSerializerConstants.ProtobufNetSerializerKey) ??
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
