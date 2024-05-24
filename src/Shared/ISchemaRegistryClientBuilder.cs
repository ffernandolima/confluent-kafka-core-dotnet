using Confluent.SchemaRegistry;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaRegistryClientBuilder
    {
        ISchemaRegistryClientBuilder WithConfigureSchemaRegistry(
            Action<ISchemaRegistryConfigBuilder> configureSchemaRegistry);

        ISchemaRegistryClientBuilder WithAuthenticationHeaderValueProvider(
            IAuthenticationHeaderValueProvider authenticationHeaderValueProvider);
    }
}
