using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.JsonCore;
using Confluent.Kafka.Core.Serialization.JsonCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonCoreSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonCoreSerializer<T>(
            this IServiceCollection services,
            Action<IServiceProvider, IJsonSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serviceKey = serializerKey ?? JsonCoreSerializerConstants.JsonCoreSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) =>
                {
                    var serializer = JsonCoreSerializerFactory.Instance.CreateSerializer<T>(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        configureOptions);

                    return serializer;
                });

            services.TryAddKeyedSingleton<ISerializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<JsonCoreSerializer<T>>(serviceKey));

            services.TryAddKeyedSingleton<IDeserializer<T>>(
                serviceKey,
                (serviceProvider, _) => serviceProvider.GetRequiredKeyedService<JsonCoreSerializer<T>>(serviceKey));

            return services;
        }
    }
}
