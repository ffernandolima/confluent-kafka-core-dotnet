using Confluent.Kafka.Core.Serialization.JsonCore;
using Confluent.Kafka.Core.Serialization.JsonCore.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonCoreSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonCoreSerializer<T>(
            this IServiceCollection services,
            Action<IJsonSerializerOptionsBuilder> configureOptions = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serializer = JsonCoreSerializerFactory.CreateSerializer<T>(configureOptions);

            services.TryAddSingleton(serializer);

            return services;
        }
    }
}
