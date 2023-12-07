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
            Action<IJsonSerializerSettingsBuilder> configureSettings = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serializer = NewtonsoftJsonSerializerFactory.CreateSerializer<T>(configureSettings);

            services.TryAddSingleton(serializer);

            return services;
        }
    }
}
