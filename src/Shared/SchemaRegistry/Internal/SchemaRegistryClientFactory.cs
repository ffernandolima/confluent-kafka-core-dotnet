using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class SchemaRegistryClientFactory : ISchemaRegistryClientFactory
    {
        private static readonly Lazy<SchemaRegistryClientFactory> Factory = new(
          () => new SchemaRegistryClientFactory(), isThreadSafe: true);

        public static SchemaRegistryClientFactory Instance => Factory.Value;

        private SchemaRegistryClientFactory()
        { }

        public ISchemaRegistryClient GetOrCreateSchemaRegistryClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey)
        {
            var schemaRegistryClient = serviceProvider?.GetKeyedService<ISchemaRegistryClient>(
                clientKey ?? SchemaRegistryClientConstants.ConfluentSchemaRegistryClientKey) ??
                CreateSchemaRegistryClient(serviceProvider, configuration, (_, builder) => configureClient?.Invoke(builder));

            return schemaRegistryClient;
        }

        public ISchemaRegistryClient CreateSchemaRegistryClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryClientBuilder> configureClient)
        {
            var builder = SchemaRegistryClientBuilder.Configure(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureClient);

            var schemaRegistryClient = new CachedSchemaRegistryClient(
                builder.SchemaRegistryConfig,
                builder.AuthenticationHeaderValueProvider);

            return schemaRegistryClient;
        }
    }
}
