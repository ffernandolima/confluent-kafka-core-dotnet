using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class SchemaRegistryConfigBuilder :
        FunctionalBuilder<SchemaRegistryConfig, SchemaRegistryConfigBuilder>,
        ISchemaRegistryConfigBuilder
    {
        public ISchemaRegistryConfigBuilder WithBasicAuthCredentialsSource(AuthCredentialsSource? basicAuthCredentialsSource)
        {
            AppendAction(config => config.BasicAuthCredentialsSource = basicAuthCredentialsSource);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithUrl(string url)
        {
            AppendAction(config => config.Url = url);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithRequestTimeoutMs(int? requestTimeoutMs)
        {
            AppendAction(config => config.RequestTimeoutMs = requestTimeoutMs);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithSslCaLocation(string sslCaLocation)
        {
            AppendAction(config => config.SslCaLocation = sslCaLocation);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithSslKeystoreLocation(string sslKeystoreLocation)
        {
            AppendAction(config => config.SslKeystoreLocation = sslKeystoreLocation);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithSslKeystorePassword(string sslKeystorePassword)
        {
            AppendAction(config => config.SslKeystorePassword = sslKeystorePassword);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithEnableSslCertificateVerification(bool? enableSslCertificateVerification)
        {
            AppendAction(config => config.EnableSslCertificateVerification = enableSslCertificateVerification);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithMaxCachedSchemas(int? maxCachedSchemas)
        {
            AppendAction(config => config.MaxCachedSchemas = maxCachedSchemas);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBasicAuthUserInfo(string basicAuthUserInfo)
        {
            AppendAction(config => config.BasicAuthUserInfo = basicAuthUserInfo);
            return this;
        }

        public static SchemaRegistryConfig Build(Action<ISchemaRegistryConfigBuilder> configureSchemaRegistry)
        {
            using var builder = new SchemaRegistryConfigBuilder();

            configureSchemaRegistry?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
