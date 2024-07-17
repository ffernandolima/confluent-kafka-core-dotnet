using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal sealed class ProtobufNetSerializerFactory : IProtobufNetSerializerFactory
    {
        private static readonly Lazy<ProtobufNetSerializerFactory> Factory = new(
          () => new ProtobufNetSerializerFactory(), isThreadSafe: true);

        public static ProtobufNetSerializerFactory Instance => Factory.Value;

        private ProtobufNetSerializerFactory()
        { }

        public ProtobufNetSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IProtobufNetSerializerOptionsBuilder> configureOptions,
            object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<ProtobufNetSerializer<T>>(
                serializerKey ?? ProtobufNetSerializerConstants.ProtobufNetSerializerKey) ??
                CreateSerializer<T>(serviceProvider, configuration, (_, builder) => configureOptions?.Invoke(builder));

            return serializer;
        }

        public ProtobufNetSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions)
        {
            var options = ProtobufNetSerializerOptionsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureOptions);

            var serializer = new ProtobufNetSerializer<T>(options);

            return serializer;
        }
    }
}
