using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NewtonsoftJsonSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddNewtonsoftJsonSerializer<T>(
            this IServiceCollection services,
            Action<IServiceProvider, IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serviceKey = serializerKey ?? NewtonsoftJsonSerializerConstants.NewtonsoftJsonSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) =>
                {
                    var serializer = NewtonsoftJsonSerializerFactory.Instance.CreateSerializer<T>(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        configureSettings);

                    return serializer;
                });

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
