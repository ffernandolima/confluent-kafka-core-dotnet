using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class SchemaRegistryAvroSerializerFactory : ISchemaRegistryAvroSerializerFactory
    {
        private static readonly Lazy<SchemaRegistryAvroSerializerFactory> Factory = new(
          () => new SchemaRegistryAvroSerializerFactory(), isThreadSafe: true);

        public static SchemaRegistryAvroSerializerFactory Instance => Factory.Value;

        private SchemaRegistryAvroSerializerFactory()
        { }

        public SchemaRegistryAvroSerializer<T> GetOrCreateSerializer<T>(
           IServiceProvider serviceProvider,
           IConfiguration configuration,
           Action<ISchemaRegistryAvroSerializerBuilder> configureSerializer,
           object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryAvroSerializer<T>>(
                serializerKey ?? SchemaRegistryAvroSerializerConstants.SchemaRegistryAvroSerializerKey) ??
                CreateSerializer<T>(serviceProvider, configuration, (_, builder) => configureSerializer?.Invoke(builder));

            return serializer;
        }

        public SchemaRegistryAvroSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryAvroSerializerBuilder> configureSerializer)
        {
            var builder = SchemaRegistryAvroSerializerBuilder.Configure(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureSerializer);

            var serializer = new SchemaRegistryAvroSerializer<T>(
                builder.SchemaRegistryClient,
                builder.SerializerConfig,
                builder.DeserializerConfig);

            return serializer;
        }
    }
}
