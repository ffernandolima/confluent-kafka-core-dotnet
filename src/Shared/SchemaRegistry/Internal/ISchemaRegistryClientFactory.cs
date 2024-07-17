using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal interface ISchemaRegistryClientFactory
    {
        ISchemaRegistryClient GetOrCreateSchemaRegistryClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<ISchemaRegistryClientBuilder> configureClient,
            object clientKey);

        ISchemaRegistryClient CreateSchemaRegistryClient(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryClientBuilder> configureClient);
    }
}
