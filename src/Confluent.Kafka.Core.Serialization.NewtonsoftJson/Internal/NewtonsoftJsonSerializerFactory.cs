﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal
{
    internal static class NewtonsoftJsonSerializerFactory
    {
        public static NewtonsoftJsonSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            Action<IJsonSerializerSettingsBuilder> configureSettings,
            object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<NewtonsoftJsonSerializer<T>>(
                serializerKey ?? NewtonsoftJsonSerializerConstants.NewtonsoftJsonSerializerKey) ??
                CreateSerializer<T>(configureSettings);

            return serializer;
        }

        public static NewtonsoftJsonSerializer<T> CreateSerializer<T>(
            Action<IJsonSerializerSettingsBuilder> configureSettings)
        {
            var settings = JsonSerializerSettingsBuilder.Build(configureSettings);

            var serializer = new NewtonsoftJsonSerializer<T>(settings);

            return serializer;
        }
    }
}
