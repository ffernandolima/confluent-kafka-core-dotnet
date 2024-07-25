namespace Confluent.Kafka.Core.Client
{
    public interface IClientConfig : IConfig
    {
        SaslMechanism? SaslMechanism { get; }

        Acks? Acks { get; }

        string ClientId { get; }

        string BootstrapServers { get; }

        int? MessageMaxBytes { get; }

        int? MessageCopyMaxBytes { get; }

        int? ReceiveMessageMaxBytes { get; }

        int? MaxInFlight { get; }

        int? TopicMetadataRefreshIntervalMs { get; }

        int? MetadataMaxAgeMs { get; }

        int? TopicMetadataRefreshFastIntervalMs { get; }

        bool? TopicMetadataRefreshSparse { get; }

        int? TopicMetadataPropagationMaxMs { get; }

        string TopicBlacklist { get; }

        string Debug { get; }

        int? SocketTimeoutMs { get; }

        int? SocketSendBufferBytes { get; }

        int? SocketReceiveBufferBytes { get; }

        bool? SocketKeepaliveEnable { get; }

        bool? SocketNagleDisable { get; }

        int? SocketMaxFails { get; }

        int? BrokerAddressTtl { get; }

        BrokerAddressFamily? BrokerAddressFamily { get; }

        int? SocketConnectionSetupTimeoutMs { get; }

        int? ConnectionsMaxIdleMs { get; }

        int? ReconnectBackoffMs { get; }

        int? ReconnectBackoffMaxMs { get; }

        int? StatisticsIntervalMs { get; }

        bool? LogQueue { get; }

        bool? LogThreadName { get; }

        bool? EnableRandomSeed { get; }

        bool? LogConnectionClose { get; }

        int? InternalTerminationSignal { get; }

        bool? ApiVersionRequest { get; }

        int? ApiVersionRequestTimeoutMs { get; }

        int? ApiVersionFallbackMs { get; }

        string BrokerVersionFallback { get; }

        bool? AllowAutoCreateTopics { get; }

        SecurityProtocol? SecurityProtocol { get; }

        string SslCipherSuites { get; }

        string SslCurvesList { get; }

        string SslSigalgsList { get; }

        string SslKeyLocation { get; }

        string SslKeyPassword { get; }

        string SslKeyPem { get; }

        string SslCertificateLocation { get; }

        string SslCertificatePem { get; }

        string SslCaLocation { get; }

        string SslCaPem { get; }

        string SslCaCertificateStores { get; }

        string SslCrlLocation { get; }

        string SslKeystoreLocation { get; }

        string SslKeystorePassword { get; }

        string SslProviders { get; }

        string SslEngineLocation { get; }

        string SslEngineId { get; }

        bool? EnableSslCertificateVerification { get; }

        SslEndpointIdentificationAlgorithm? SslEndpointIdentificationAlgorithm { get; }

        string SaslKerberosServiceName { get; }

        string SaslKerberosPrincipal { get; }

        string SaslKerberosKinitCmd { get; }

        string SaslKerberosKeytab { get; }

        int? SaslKerberosMinTimeBeforeRelogin { get; }

        string SaslUsername { get; }

        string SaslPassword { get; }

        string SaslOauthbearerConfig { get; }

        bool? EnableSaslOauthbearerUnsecureJwt { get; }

        SaslOauthbearerMethod? SaslOauthbearerMethod { get; }

        string SaslOauthbearerClientId { get; }

        string SaslOauthbearerClientSecret { get; }

        string SaslOauthbearerScope { get; }

        string SaslOauthbearerExtensions { get; }

        string SaslOauthbearerTokenEndpointUrl { get; }

        string PluginLibraryPaths { get; }

        string ClientRack { get; }

        public int? RetryBackoffMs { get; }

        public int? RetryBackoffMaxMs { get; }

        ClientDnsLookup? ClientDnsLookup { get; }

        public bool? EnableMetricsPush { get; }
    }
}
