using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class SchemaRegistryJsonSerializerFactory
    {
        private static readonly Lazy<SchemaRegistryJsonSerializerFactory> Factory = new(
          () => new SchemaRegistryJsonSerializerFactory(), isThreadSafe: true);

        public static SchemaRegistryJsonSerializerFactory Instance => Factory.Value;

        private SchemaRegistryJsonSerializerFactory()
        { }

        public SchemaRegistryJsonSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<ISchemaRegistryJsonSerializerBuilder> configureSerializer,
            object serializerKey)
                where T : class
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryJsonSerializer<T>>(
                serializerKey ?? SchemaRegistryJsonSerializerConstants.SchemaRegistryJsonSerializerKey) ??
                CreateSerializer<T>(serviceProvider, configuration, (_, builder) => configureSerializer?.Invoke(builder));

            return serializer;
        }

        public SchemaRegistryJsonSerializer<T> CreateSerializer<T>(
           IServiceProvider serviceProvider,
           IConfiguration configuration,
           Action<IServiceProvider, ISchemaRegistryJsonSerializerBuilder> configureSerializer)
                where T : class
        {
            var builder = SchemaRegistryJsonSerializerBuilder.Configure(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureSerializer);

            var serializer = new SchemaRegistryJsonSerializer<T>(
                builder.SchemaRegistryClient,
                builder.RegisteredSchema ?? builder.UnregisteredSchema,
                builder.SerializerConfig,
                builder.DeserializerConfig,
                builder.SchemaGeneratorSettings);

            return serializer;
        }
    }
}
