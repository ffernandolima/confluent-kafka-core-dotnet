using Confluent.SchemaRegistry;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaRegistryClientBuilder
    {
        ISchemaRegistryClientBuilder WithSchemaRegistryConfiguration(
            Action<ISchemaRegistryConfigBuilder> configureSchemaRegistry);

        ISchemaRegistryClientBuilder WithAuthenticationHeaderValueProvider(
            IAuthenticationHeaderValueProvider authenticationHeaderValueProvider);
    }
}
