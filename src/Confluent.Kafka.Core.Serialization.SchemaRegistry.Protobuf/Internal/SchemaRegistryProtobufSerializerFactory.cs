using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class SchemaRegistryProtobufSerializerFactory : ISchemaRegistryProtobufSerializerFactory
    {
        private static readonly Lazy<SchemaRegistryProtobufSerializerFactory> Factory = new(
          () => new SchemaRegistryProtobufSerializerFactory(), isThreadSafe: true);

        public static SchemaRegistryProtobufSerializerFactory Instance => Factory.Value;

        private SchemaRegistryProtobufSerializerFactory()
        { }

        public SchemaRegistryProtobufSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<ISchemaRegistryProtobufSerializerBuilder> configureSerializer,
            object serializerKey)
                where T : class, IMessage<T>, new()
        {
            var serializer = serviceProvider?.GetKeyedService<SchemaRegistryProtobufSerializer<T>>(
                serializerKey ?? SchemaRegistryProtobufSerializerConstants.SchemaRegistryProtobufSerializerKey) ??
                CreateSerializer<T>(serviceProvider, configuration, (_, builder) => configureSerializer?.Invoke(builder));

            return serializer;
        }

        public SchemaRegistryProtobufSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryProtobufSerializerBuilder> configureSerializer)
                where T : class, IMessage<T>, new()
        {
            var builder = SchemaRegistryProtobufSerializerBuilder.Configure(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureSerializer);

            var serializer = new SchemaRegistryProtobufSerializer<T>(
                builder.SchemaRegistryClient,
                builder.SerializerConfig,
                builder.DeserializerConfig);

            return serializer;
        }
    }
}
