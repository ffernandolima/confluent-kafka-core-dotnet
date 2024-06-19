﻿using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Confluent.Kafka.Core.Serialization.ProtobufNet.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProtobufNetSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddProtobufNetSerializer<T>(
            this IServiceCollection services,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serviceKey = serializerKey ?? ProtobufNetSerializerConstants.ProtobufNetSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) => ProtobufNetSerializerFactory.CreateSerializer<T>(serviceProvider, configureOptions));

            services.TryAddKeyedSingleton<ISerializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<ProtobufNetSerializer<T>>(serviceKey));

            services.TryAddKeyedSingleton<IDeserializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<ProtobufNetSerializer<T>>(serviceKey));

            return services;
        }
    }
}
