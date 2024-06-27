using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class SchemaRegistryClientBuilder : ISchemaRegistryClientBuilder
    {
        private readonly IConfiguration _configuration;

        public SchemaRegistryConfig SchemaRegistryConfig { get; private set; }
        public IAuthenticationHeaderValueProvider AuthenticationHeaderValueProvider { get; private set; }

        public SchemaRegistryClientBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ISchemaRegistryClientBuilder WithSchemaRegistryConfiguration(
            Action<ISchemaRegistryConfigBuilder> configureSchemaRegistry)
        {
            SchemaRegistryConfig = SchemaRegistryConfigBuilder.Build(_configuration, configureSchemaRegistry);
            return this;
        }

        public ISchemaRegistryClientBuilder WithAuthenticationHeaderValueProvider(
            IAuthenticationHeaderValueProvider authenticationHeaderValueProvider)
        {
            AuthenticationHeaderValueProvider = authenticationHeaderValueProvider;
            return this;
        }

        public static SchemaRegistryClientBuilder Configure(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, ISchemaRegistryClientBuilder> configureClient)
        {
            var builder = new SchemaRegistryClientBuilder(configuration);

            configureClient?.Invoke(serviceProvider, builder);

            return builder;
        }
    }
}
