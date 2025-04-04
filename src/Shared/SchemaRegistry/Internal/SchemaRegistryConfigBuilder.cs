using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class SchemaRegistryConfigBuilder :
        FunctionalBuilder<SchemaRegistryConfig, SchemaRegistryConfigBuilder>,
        ISchemaRegistryConfigBuilder
    {
        public SchemaRegistryConfigBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public ISchemaRegistryConfigBuilder FromConfiguration(string sectionKey)
        {
            AppendAction(config =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    config = Bind(config, sectionKey);
                }
            });
            return this;
        }

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

        public ISchemaRegistryConfigBuilder WithMaxRetries(int? maxRetries)
        {
            AppendAction(config => config.MaxRetries = maxRetries);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithRetriesWaitMs(int? retriesWaitMs)
        {
            AppendAction(config => config.RetriesWaitMs = retriesWaitMs);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithRetriesMaxWaitMs(int? retriesMaxWaitMs)
        {
            AppendAction(config => config.RetriesMaxWaitMs = retriesMaxWaitMs);
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

        public ISchemaRegistryConfigBuilder WithLatestCacheTtlSecs(int? latestCacheTtlSecs)
        {
            AppendAction(config => config.LatestCacheTtlSecs = latestCacheTtlSecs);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBasicAuthUserInfo(string basicAuthUserInfo)
        {
            AppendAction(config => config.BasicAuthUserInfo = basicAuthUserInfo);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthCredentialsSource(BearerAuthCredentialsSource? bearerAuthCredentialsSource)
        {
            AppendAction(config => config.BearerAuthCredentialsSource = bearerAuthCredentialsSource);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthToken(string bearerAuthToken)
        {
            AppendAction(config => config.BearerAuthToken = bearerAuthToken);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthLogicalCluster(string bearerAuthLogicalCluster)
        {
            AppendAction(config => config.BearerAuthLogicalCluster = bearerAuthLogicalCluster);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthIdentityPoolId(string bearerAuthIdentityPoolId)
        {
            AppendAction(config => config.BearerAuthIdentityPoolId = bearerAuthIdentityPoolId);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthClientId(string bearerAuthClientId)
        {
            AppendAction(config => config.BearerAuthClientId = bearerAuthClientId);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthClientSecret(string bearerAuthClientSecret)
        {
            AppendAction(config => config.BearerAuthClientSecret = bearerAuthClientSecret);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthScope(string bearerAuthScope)
        {
            AppendAction(config => config.BearerAuthScope = bearerAuthScope);
            return this;
        }

        public ISchemaRegistryConfigBuilder WithBearerAuthTokenEndpointUrl(string bearerAuthTokenEndpointUrl)
        {
            AppendAction(config => config.BearerAuthTokenEndpointUrl = bearerAuthTokenEndpointUrl);
            return this;
        }

        public static SchemaRegistryConfig Build(
            IConfiguration configuration,
            Action<ISchemaRegistryConfigBuilder> configureSchemaRegistry)
        {
            using var builder = new SchemaRegistryConfigBuilder(configuration);

            configureSchemaRegistry?.Invoke(builder);

            var config = builder.Build();

            return config;
        }
    }
}
