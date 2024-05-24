using Confluent.SchemaRegistry;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class SchemaRegistryClientBuilder : ISchemaRegistryClientBuilder
    {
        public SchemaRegistryConfig SchemaRegistryConfig { get; private set; }
        public IAuthenticationHeaderValueProvider AuthenticationHeaderValueProvider { get; private set; }

        public ISchemaRegistryClientBuilder WithConfigureSchemaRegistry(
            Action<ISchemaRegistryConfigBuilder> configureSchemaRegistry)
        {
            SchemaRegistryConfig = SchemaRegistryConfigBuilder.Build(configureSchemaRegistry);
            return this;
        }

        public ISchemaRegistryClientBuilder WithAuthenticationHeaderValueProvider(
            IAuthenticationHeaderValueProvider authenticationHeaderValueProvider)
        {
            AuthenticationHeaderValueProvider = authenticationHeaderValueProvider;
            return this;
        }

        public static SchemaRegistryClientBuilder Configure(Action<ISchemaRegistryClientBuilder> configureClient)
        {
            var builder = new SchemaRegistryClientBuilder();

            configureClient?.Invoke(builder);

            return builder;
        }
    }
}
