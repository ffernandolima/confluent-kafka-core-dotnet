using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal static class SchemaRegistryClientFactory
    {
        public static ISchemaRegistryClient GetOrCreateSchemaRegistryClient(
            IServiceProvider serviceProvider,
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey)
        {
            var schemaRegistryClient = serviceProvider?.GetKeyedService<ISchemaRegistryClient>(
                clientKey ?? SchemaRegistryClientConstants.ConfluentSchemaRegistryClientKey) ??
                CreateSchemaRegistryClient(serviceProvider, (_, builder) => configureClient?.Invoke(builder));

            return schemaRegistryClient;
        }

        public static ISchemaRegistryClient CreateSchemaRegistryClient(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, ISchemaRegistryClientBuilder> configureClient)
        {
            var builder = SchemaRegistryClientBuilder.Configure(serviceProvider, configureClient);

            var schemaRegistryClient = new CachedSchemaRegistryClient(
                builder.SchemaRegistryConfig,
                builder.AuthenticationHeaderValueProvider);

            return schemaRegistryClient;
        }
    }
}
