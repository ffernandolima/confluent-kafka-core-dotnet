using Confluent.Kafka;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaRegistryProtobufSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaRegistryProtobufSerializer<T>(
            this IServiceCollection services,
            Action<IServiceProvider, ISchemaRegistryProtobufSerializerBuilder> configureSerializer,
            object serializerKey = null)
                where T : class, IMessage<T>, new()
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureSerializer is null)
            {
                throw new ArgumentNullException(nameof(configureSerializer));
            }

            var serviceKey = serializerKey ?? SchemaRegistryProtobufSerializerConstants.SchemaRegistryProtobufSerializerKey;

            services.TryAddKeyedSingleton(
                serviceKey,
                (serviceProvider, _) =>
                {
                    var serializer = SchemaRegistryProtobufSerializerFactory.Instance.CreateSerializer<T>(
                        serviceProvider,
                        serviceProvider.GetService<IConfiguration>(),
                        configureSerializer);

                    return serializer;
                });

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
