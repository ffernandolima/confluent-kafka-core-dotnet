using Confluent.SchemaRegistry;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaRegistryConfigBuilder
    {
        ISchemaRegistryConfigBuilder WithBasicAuthCredentialsSource(AuthCredentialsSource? basicAuthCredentialsSource);

        ISchemaRegistryConfigBuilder WithUrl(string url);

        ISchemaRegistryConfigBuilder WithRequestTimeoutMs(int? requestTimeoutMs);

        ISchemaRegistryConfigBuilder WithSslCaLocation(string sslCaLocation);

        ISchemaRegistryConfigBuilder WithSslKeystoreLocation(string sslKeystoreLocation);

        ISchemaRegistryConfigBuilder WithSslKeystorePassword(string sslKeystorePassword);

        ISchemaRegistryConfigBuilder WithEnableSslCertificateVerification(bool? enableSslCertificateVerification);

        ISchemaRegistryConfigBuilder WithMaxCachedSchemas(int? maxCachedSchemas);

        ISchemaRegistryConfigBuilder WithBasicAuthUserInfo(string basicAuthUserInfo);
    }
}
