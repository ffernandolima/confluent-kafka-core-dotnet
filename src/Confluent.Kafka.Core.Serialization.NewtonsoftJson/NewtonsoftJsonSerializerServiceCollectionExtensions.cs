using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NewtonsoftJsonSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddNewtonsoftJsonSerializer<T>(
            this IServiceCollection services,
            Action<IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serviceKey = serializerKey ?? NewtonsoftJsonSerializerConstants.NewtonsoftJsonSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (_, _) => NewtonsoftJsonSerializerFactory.CreateSerializer<T>(configureSettings));

            services.TryAddKeyedSingleton<ISerializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<NewtonsoftJsonSerializer<T>>(serviceKey));

            services.TryAddKeyedSingleton<IDeserializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<NewtonsoftJsonSerializer<T>>(serviceKey));

            return services;
        }
    }
}
