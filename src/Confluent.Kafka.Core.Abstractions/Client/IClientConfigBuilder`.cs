namespace Confluent.Kafka.Core.Client
{
    public interface IClientConfigBuilder<TBuilder> :
        IConfigBuilder<TBuilder>
        where TBuilder : IClientConfigBuilder<TBuilder>
    {
        TBuilder WithSaslMechanism(SaslMechanism? saslMechanism);

        TBuilder WithAcks(Acks? acks);

        TBuilder WithClientId(string clientId);

        TBuilder WithBootstrapServers(string bootstrapServers);

        TBuilder WithMessageMaxBytes(int? messageMaxBytes);

        TBuilder WithMessageCopyMaxBytes(int? messageCopyMaxBytes);

        TBuilder WithReceiveMessageMaxBytes(int? receiveMessageMaxBytes);

        TBuilder WithMaxInFlight(int? maxInFlight);

        TBuilder WithTopicMetadataRefreshIntervalMs(int? topicMetadataRefreshIntervalMs);

        TBuilder WithMetadataMaxAgeMs(int? metadataMaxAgeMs);

        TBuilder WithTopicMetadataRefreshFastIntervalMs(int? topicMetadataRefreshFastIntervalMs);

        TBuilder WithTopicMetadataRefreshSparse(bool? topicMetadataRefreshSparse);

        TBuilder WithTopicMetadataPropagationMaxMs(int? topicMetadataPropagationMaxMs);

        TBuilder WithTopicBlacklist(string topicBlacklist);

        TBuilder WithDebug(string debug);

        TBuilder WithSocketTimeoutMs(int? socketTimeoutMs);

        TBuilder WithSocketSendBufferBytes(int? socketSendBufferBytes);

        TBuilder WithSocketReceiveBufferBytes(int? socketReceiveBufferBytes);

        TBuilder WithSocketKeepaliveEnable(bool? socketKeepaliveEnable);

        TBuilder WithSocketNagleDisable(bool? socketNagleDisable);

        TBuilder WithSocketMaxFails(int? socketMaxFails);

        TBuilder WithBrokerAddressTtl(int? brokerAddressTtl);

        TBuilder WithBrokerAddressFamily(BrokerAddressFamily? brokerAddressFamily);

        TBuilder WithSocketConnectionSetupTimeoutMs(int? socketConnectionSetupTimeoutMs);

        TBuilder WithConnectionsMaxIdleMs(int? connectionsMaxIdleMs);

        TBuilder WithReconnectBackoffMs(int? reconnectBackoffMs);

        TBuilder WithReconnectBackoffMaxMs(int? reconnectBackoffMaxMs);

        TBuilder WithStatisticsIntervalMs(int? statisticsIntervalMs);

        TBuilder WithLogQueue(bool? logQueue);

        TBuilder WithLogThreadName(bool? logThreadName);

        TBuilder WithEnableRandomSeed(bool? enableRandomSeed);

        TBuilder WithLogConnectionClose(bool? logConnectionClose);

        TBuilder WithInternalTerminationSignal(int? internalTerminationSignal);

        TBuilder WithApiVersionRequest(bool? apiVersionRequest);

        TBuilder WithApiVersionRequestTimeoutMs(int? apiVersionRequestTimeoutMs);

        TBuilder WithApiVersionFallbackMs(int? apiVersionFallbackMs);

        TBuilder WithBrokerVersionFallback(string brokerVersionFallback);

        TBuilder WithAllowAutoCreateTopics(bool? allowAutoCreateTopics);

        TBuilder WithSecurityProtocol(SecurityProtocol? securityProtocol);

        TBuilder WithSslCipherSuites(string sslCipherSuites);

        TBuilder WithSslCurvesList(string sslCurvesList);

        TBuilder WithSslSigalgsList(string sslSigalgsList);

        TBuilder WithSslKeyLocation(string sslKeyLocation);

        TBuilder WithSslKeyPassword(string sslKeyPassword);

        TBuilder WithSslKeyPem(string sslKeyPem);

        TBuilder WithSslCertificateLocation(string sslCertificateLocation);

        TBuilder WithSslCertificatePem(string sslCertificatePem);

        TBuilder WithSslCaLocation(string sslCaLocation);

        TBuilder WithSslCaPem(string sslCaPem);

        TBuilder WithSslCaCertificateStores(string sslCaCertificateStores);

        TBuilder WithSslCrlLocation(string sslCrlLocation);

        TBuilder WithSslKeystoreLocation(string sslKeystoreLocation);

        TBuilder WithSslKeystorePassword(string sslKeystorePassword);

        TBuilder WithSslProviders(string sslProviders);

        TBuilder WithSslEngineLocation(string sslEngineLocation);

        TBuilder WithSslEngineId(string sslEngineId);

        TBuilder WithEnableSslCertificateVerification(bool? enableSslCertificateVerification);

        TBuilder WithSslEndpointIdentificationAlgorithm(SslEndpointIdentificationAlgorithm? sslEndpointIdentificationAlgorithm);

        TBuilder WithSaslKerberosServiceName(string saslKerberosServiceName);

        TBuilder WithSaslKerberosPrincipal(string saslKerberosPrincipal);

        TBuilder WithSaslKerberosKinitCmd(string saslKerberosKinitCmd);

        TBuilder WithSaslKerberosKeytab(string saslKerberosKeytab);

        TBuilder WithSaslKerberosMinTimeBeforeRelogin(int? saslKerberosMinTimeBeforeRelogin);

        TBuilder WithSaslUsername(string saslUsername);

        TBuilder WithSaslPassword(string saslPassword);

        TBuilder WithSaslOauthbearerConfig(string saslOauthbearerConfig);

        TBuilder WithEnableSaslOauthbearerUnsecureJwt(bool? enableSaslOauthbearerUnsecureJwt);

        TBuilder WithSaslOauthbearerMethod(SaslOauthbearerMethod? saslOauthbearerMethod);

        TBuilder WithSaslOauthbearerClientId(string saslOauthbearerClientId);

        TBuilder WithSaslOauthbearerClientSecret(string saslOauthbearerClientSecret);

        TBuilder WithSaslOauthbearerScope(string saslOauthbearerScope);

        TBuilder WithSaslOauthbearerExtensions(string saslOauthbearerExtensions);

        TBuilder WithSaslOauthbearerTokenEndpointUrl(string saslOauthbearerTokenEndpointUrl);

        TBuilder WithPluginLibraryPaths(string pluginLibraryPaths);

        TBuilder WithClientRack(string clientRack);

        TBuilder WithRetryBackoffMs(int? retryBackoffMs);

        TBuilder WithRetryBackoffMaxMs(int? retryBackoffMaxMs);

        TBuilder WithClientDnsLookup(ClientDnsLookup? clientDnsLookup);

        TBuilder WithEnableMetricsPush(bool? enableMetricsPush);
    }
}
