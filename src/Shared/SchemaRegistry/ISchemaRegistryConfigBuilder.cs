using Confluent.SchemaRegistry;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaRegistryConfigBuilder
    {
        ISchemaRegistryConfigBuilder FromConfiguration(string sectionKey);

        ISchemaRegistryConfigBuilder WithBasicAuthCredentialsSource(AuthCredentialsSource? basicAuthCredentialsSource);

        ISchemaRegistryConfigBuilder WithUrl(string url);

        ISchemaRegistryConfigBuilder WithRequestTimeoutMs(int? requestTimeoutMs);

        ISchemaRegistryConfigBuilder WithMaxRetries(int? maxRetries);

        ISchemaRegistryConfigBuilder WithRetriesWaitMs(int? retriesWaitMs);

        ISchemaRegistryConfigBuilder WithRetriesMaxWaitMs(int? retriesMaxWaitMs);

        ISchemaRegistryConfigBuilder WithSslCaLocation(string sslCaLocation);

        ISchemaRegistryConfigBuilder WithSslKeystoreLocation(string sslKeystoreLocation);

        ISchemaRegistryConfigBuilder WithSslKeystorePassword(string sslKeystorePassword);

        ISchemaRegistryConfigBuilder WithEnableSslCertificateVerification(bool? enableSslCertificateVerification);

        ISchemaRegistryConfigBuilder WithMaxCachedSchemas(int? maxCachedSchemas);

        ISchemaRegistryConfigBuilder WithLatestCacheTtlSecs(int? latestCacheTtlSecs);

        ISchemaRegistryConfigBuilder WithBasicAuthUserInfo(string basicAuthUserInfo);

        ISchemaRegistryConfigBuilder WithBearerAuthCredentialsSource(BearerAuthCredentialsSource? bearerAuthCredentialsSource);

        ISchemaRegistryConfigBuilder WithBearerAuthToken(string bearerAuthToken);

        ISchemaRegistryConfigBuilder WithBearerAuthLogicalCluster(string bearerAuthLogicalCluster);

        ISchemaRegistryConfigBuilder WithBearerAuthIdentityPoolId(string bearerAuthIdentityPoolId);

        ISchemaRegistryConfigBuilder WithBearerAuthClientId(string bearerAuthClientId);

        ISchemaRegistryConfigBuilder WithBearerAuthClientSecret(string bearerAuthClientSecret);

        ISchemaRegistryConfigBuilder WithBearerAuthScope(string bearerAuthScope);

        ISchemaRegistryConfigBuilder WithBearerAuthTokenEndpointUrl(string bearerAuthTokenEndpointUrl);
    }
}
