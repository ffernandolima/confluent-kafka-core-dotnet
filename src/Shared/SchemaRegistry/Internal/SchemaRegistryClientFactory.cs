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
            object clientKey = null)
        {
            var schemaRegistryClient = serviceProvider?.GetKeyedService<ISchemaRegistryClient>(
                clientKey ?? SchemaRegistryClientConstants.ConfluentSchemaRegistryClientKey) ??
                CreateSchemaRegistryClient(configureClient);

            return schemaRegistryClient;
        }

        public static ISchemaRegistryClient CreateSchemaRegistryClient(
            Action<ISchemaRegistryClientBuilder> configureClient)
        {
            if (configureClient is null)
            {
                throw new ArgumentNullException(nameof(configureClient), $"{nameof(configureClient)} cannot be null.");
            }

            var builder = SchemaRegistryClientBuilder.Configure(configureClient);

            var schemaRegistryConfig = builder.SchemaRegistryConfig;

            if (schemaRegistryConfig is null)
            {
                throw new InvalidOperationException($"{nameof(schemaRegistryConfig)} cannot be null.");
            }

            var schemaRegistryClient = new CachedSchemaRegistryClient(
                schemaRegistryConfig,
                builder.AuthenticationHeaderValueProvider);

            return schemaRegistryClient;
        }
    }
}
