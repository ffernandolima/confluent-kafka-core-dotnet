﻿using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaRegistryProtobufSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaRegistryProtobufSerializer<T>(
            this IServiceCollection services,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer,
            object serializerKey = null)
                where T : class, IMessage<T>, new()
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer), $"{nameof(configureSerializer)} cannot be null.");
            }

            var builder = SchemaRegistryProtobufSerializerBuilder.Configure(configureSerializer);

            services.AddSchemaRegistryClient(builder.ConfigureClient, builder.ClientKey);

            var serviceKey = serializerKey ?? SchemaRegistryProtobufSerializerConstants.SchemaRegistryProtobufSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) => SchemaRegistryProtobufSerializerFactory.CreateSerializer<T>(serviceProvider, builder));

            services.TryAddKeyedSingleton<IAsyncSerializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<SchemaRegistryProtobufSerializer<T>>(serviceKey));

            services.TryAddKeyedSingleton<IAsyncDeserializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<SchemaRegistryProtobufSerializer<T>>(serviceKey));

            return services;
        }
    }
}
