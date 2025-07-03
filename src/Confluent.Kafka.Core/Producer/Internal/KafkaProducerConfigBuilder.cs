using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerConfigBuilder :
        FunctionalBuilder<KafkaProducerConfig, IKafkaProducerConfig, KafkaProducerConfigBuilder>,
        IKafkaProducerConfigBuilder
    {
        public KafkaProducerConfigBuilder(IKafkaProducerConfig producerConfig = null, IConfiguration configuration = null)
            : base(producerConfig, configuration)
        { }

        #region IConfigBuilder Members

        public IKafkaProducerConfigBuilder WithCancellationDelayMaxMs(int cancellationDelayMaxMs)
        {
            AppendAction(config => config.CancellationDelayMaxMs = cancellationDelayMaxMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithConfigProperty(KeyValuePair<string, string> configProperty)
        {
            AppendAction(config => config.Set(configProperty.Key, configProperty.Value));
            return this;
        }

        public IKafkaProducerConfigBuilder WithConfigProperty(string configPropertyKey, string configPropertyValue)
        {
            AppendAction(config => config.Set(configPropertyKey, configPropertyValue));
            return this;
        }

        #endregion IConfigBuilder Members

        #region IClientConfigBuilder Members

        public IKafkaProducerConfigBuilder WithSaslMechanism(SaslMechanism? saslMechanism)
        {
            AppendAction(config => config.SaslMechanism = saslMechanism);
            return this;
        }

        public IKafkaProducerConfigBuilder WithAcks(Acks? acks)
        {
            AppendAction(config => config.Acks = acks);
            return this;
        }

        public IKafkaProducerConfigBuilder WithClientId(string clientId)
        {
            AppendAction(config => config.ClientId = clientId);
            return this;
        }

        public IKafkaProducerConfigBuilder WithBootstrapServers(string bootstrapServers)
        {
            AppendAction(config => config.BootstrapServers = bootstrapServers);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMessageMaxBytes(int? messageMaxBytes)
        {
            AppendAction(config => config.MessageMaxBytes = messageMaxBytes);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMessageCopyMaxBytes(int? messageCopyMaxBytes)
        {
            AppendAction(config => config.MessageCopyMaxBytes = messageCopyMaxBytes);
            return this;
        }

        public IKafkaProducerConfigBuilder WithReceiveMessageMaxBytes(int? receiveMessageMaxBytes)
        {
            AppendAction(config => config.ReceiveMessageMaxBytes = receiveMessageMaxBytes);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMaxInFlight(int? maxInFlight)
        {
            AppendAction(config => config.MaxInFlight = maxInFlight);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMetadataRecoveryStrategy(MetadataRecoveryStrategy? metadataRecoveryStrategy)
        {
            AppendAction(config => config.MetadataRecoveryStrategy = metadataRecoveryStrategy);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMetadataRecoveryRebootstrapTriggerMs(int? metadataRecoveryRebootstrapTriggerMs)
        {
            AppendAction(config => config.MetadataRecoveryRebootstrapTriggerMs = metadataRecoveryRebootstrapTriggerMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithTopicMetadataRefreshIntervalMs(int? topicMetadataRefreshIntervalMs)
        {
            AppendAction(config => config.TopicMetadataRefreshIntervalMs = topicMetadataRefreshIntervalMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMetadataMaxAgeMs(int? metadataMaxAgeMs)
        {
            AppendAction(config => config.MetadataMaxAgeMs = metadataMaxAgeMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithTopicMetadataRefreshFastIntervalMs(int? topicMetadataRefreshFastIntervalMs)
        {
            AppendAction(config => config.TopicMetadataRefreshFastIntervalMs = topicMetadataRefreshFastIntervalMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithTopicMetadataRefreshSparse(bool? topicMetadataRefreshSparse)
        {
            AppendAction(config => config.TopicMetadataRefreshSparse = topicMetadataRefreshSparse);
            return this;
        }

        public IKafkaProducerConfigBuilder WithTopicMetadataPropagationMaxMs(int? topicMetadataPropagationMaxMs)
        {
            AppendAction(config => config.TopicMetadataPropagationMaxMs = topicMetadataPropagationMaxMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithTopicBlacklist(string topicBlacklist)
        {
            AppendAction(config => config.TopicBlacklist = topicBlacklist);
            return this;
        }

        public IKafkaProducerConfigBuilder WithDebug(string debug)
        {
            AppendAction(config => config.Debug = debug);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSocketTimeoutMs(int? socketTimeoutMs)
        {
            AppendAction(config => config.SocketTimeoutMs = socketTimeoutMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSocketSendBufferBytes(int? socketSendBufferBytes)
        {
            AppendAction(config => config.SocketSendBufferBytes = socketSendBufferBytes);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSocketReceiveBufferBytes(int? socketReceiveBufferBytes)
        {
            AppendAction(config => config.SocketReceiveBufferBytes = socketReceiveBufferBytes);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSocketKeepaliveEnable(bool? socketKeepaliveEnable)
        {
            AppendAction(config => config.SocketKeepaliveEnable = socketKeepaliveEnable);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSocketNagleDisable(bool? socketNagleDisable)
        {
            AppendAction(config => config.SocketNagleDisable = socketNagleDisable);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSocketMaxFails(int? socketMaxFails)
        {
            AppendAction(config => config.SocketMaxFails = socketMaxFails);
            return this;
        }

        public IKafkaProducerConfigBuilder WithBrokerAddressTtl(int? brokerAddressTtl)
        {
            AppendAction(config => config.BrokerAddressTtl = brokerAddressTtl);
            return this;
        }

        public IKafkaProducerConfigBuilder WithBrokerAddressFamily(BrokerAddressFamily? brokerAddressFamily)
        {
            AppendAction(config => config.BrokerAddressFamily = brokerAddressFamily);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSocketConnectionSetupTimeoutMs(int? socketConnectionSetupTimeoutMs)
        {
            AppendAction(config => config.SocketConnectionSetupTimeoutMs = socketConnectionSetupTimeoutMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithConnectionsMaxIdleMs(int? connectionsMaxIdleMs)
        {
            AppendAction(config => config.ConnectionsMaxIdleMs = connectionsMaxIdleMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithReconnectBackoffMs(int? reconnectBackoffMs)
        {
            AppendAction(config => config.ReconnectBackoffMs = reconnectBackoffMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithReconnectBackoffMaxMs(int? reconnectBackoffMaxMs)
        {
            AppendAction(config => config.ReconnectBackoffMaxMs = reconnectBackoffMaxMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithStatisticsIntervalMs(int? statisticsIntervalMs)
        {
            AppendAction(config => config.StatisticsIntervalMs = statisticsIntervalMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithLogQueue(bool? logQueue)
        {
            AppendAction(config => config.LogQueue = logQueue);
            return this;
        }

        public IKafkaProducerConfigBuilder WithLogThreadName(bool? logThreadName)
        {
            AppendAction(config => config.LogThreadName = logThreadName);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableRandomSeed(bool? enableRandomSeed)
        {
            AppendAction(config => config.EnableRandomSeed = enableRandomSeed);
            return this;
        }

        public IKafkaProducerConfigBuilder WithLogConnectionClose(bool? logConnectionClose)
        {
            AppendAction(config => config.LogConnectionClose = logConnectionClose);
            return this;
        }

        public IKafkaProducerConfigBuilder WithInternalTerminationSignal(int? internalTerminationSignal)
        {
            AppendAction(config => config.InternalTerminationSignal = internalTerminationSignal);
            return this;
        }

        public IKafkaProducerConfigBuilder WithApiVersionRequest(bool? apiVersionRequest)
        {
            AppendAction(config => config.ApiVersionRequest = apiVersionRequest);
            return this;
        }

        public IKafkaProducerConfigBuilder WithApiVersionRequestTimeoutMs(int? apiVersionRequestTimeoutMs)
        {
            AppendAction(config => config.ApiVersionRequestTimeoutMs = apiVersionRequestTimeoutMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithApiVersionFallbackMs(int? apiVersionFallbackMs)
        {
            AppendAction(config => config.ApiVersionFallbackMs = apiVersionFallbackMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithBrokerVersionFallback(string brokerVersionFallback)
        {
            AppendAction(config => config.BrokerVersionFallback = brokerVersionFallback);
            return this;
        }

        public IKafkaProducerConfigBuilder WithAllowAutoCreateTopics(bool? allowAutoCreateTopics)
        {
            AppendAction(config => config.AllowAutoCreateTopics = allowAutoCreateTopics);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSecurityProtocol(SecurityProtocol? securityProtocol)
        {
            AppendAction(config => config.SecurityProtocol = securityProtocol);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCipherSuites(string sslCipherSuites)
        {
            AppendAction(config => config.SslCipherSuites = sslCipherSuites);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCurvesList(string sslCurvesList)
        {
            AppendAction(config => config.SslCurvesList = sslCurvesList);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslSigalgsList(string sslSigalgsList)
        {
            AppendAction(config => config.SslSigalgsList = sslSigalgsList);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslKeyLocation(string sslKeyLocation)
        {
            AppendAction(config => config.SslKeyLocation = sslKeyLocation);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslKeyPassword(string sslKeyPassword)
        {
            AppendAction(config => config.SslKeyPassword = sslKeyPassword);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslKeyPem(string sslKeyPem)
        {
            AppendAction(config => config.SslKeyPem = sslKeyPem);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCertificateLocation(string sslCertificateLocation)
        {
            AppendAction(config => config.SslCertificateLocation = sslCertificateLocation);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCertificatePem(string sslCertificatePem)
        {
            AppendAction(config => config.SslCertificatePem = sslCertificatePem);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCaLocation(string sslCaLocation)
        {
            AppendAction(config => config.SslCaLocation = sslCaLocation);
            return this;
        }

        public IKafkaProducerConfigBuilder WithHttpsCaLocation(string httpsCaLocation)
        {
            AppendAction(config => config.HttpsCaLocation = httpsCaLocation);
            return this;
        }

        public IKafkaProducerConfigBuilder WithHttpsCaPem(string httpsCaPem)
        {
            AppendAction(config => config.HttpsCaPem = httpsCaPem);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCaPem(string sslCaPem)
        {
            AppendAction(config => config.SslCaPem = sslCaPem);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCaCertificateStores(string sslCaCertificateStores)
        {
            AppendAction(config => config.SslCaCertificateStores = sslCaCertificateStores);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslCrlLocation(string sslCrlLocation)
        {
            AppendAction(config => config.SslCrlLocation = sslCrlLocation);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslKeystoreLocation(string sslKeystoreLocation)
        {
            AppendAction(config => config.SslKeystoreLocation = sslKeystoreLocation);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslKeystorePassword(string sslKeystorePassword)
        {
            AppendAction(config => config.SslKeystorePassword = sslKeystorePassword);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslProviders(string sslProviders)
        {
            AppendAction(config => config.SslProviders = sslProviders);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslEngineLocation(string sslEngineLocation)
        {
            AppendAction(config => config.SslEngineLocation = sslEngineLocation);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslEngineId(string sslEngineId)
        {
            AppendAction(config => config.SslEngineId = sslEngineId);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableSslCertificateVerification(bool? enableSslCertificateVerification)
        {
            AppendAction(config => config.EnableSslCertificateVerification = enableSslCertificateVerification);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSslEndpointIdentificationAlgorithm(SslEndpointIdentificationAlgorithm? sslEndpointIdentificationAlgorithm)
        {
            AppendAction(config => config.SslEndpointIdentificationAlgorithm = sslEndpointIdentificationAlgorithm);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslKerberosServiceName(string saslKerberosServiceName)
        {
            AppendAction(config => config.SaslKerberosServiceName = saslKerberosServiceName);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslKerberosPrincipal(string saslKerberosPrincipal)
        {
            AppendAction(config => config.SaslKerberosPrincipal = saslKerberosPrincipal);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslKerberosKinitCmd(string saslKerberosKinitCmd)
        {
            AppendAction(config => config.SaslKerberosKinitCmd = saslKerberosKinitCmd);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslKerberosKeytab(string saslKerberosKeytab)
        {
            AppendAction(config => config.SaslKerberosKeytab = saslKerberosKeytab);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslKerberosMinTimeBeforeRelogin(int? saslKerberosMinTimeBeforeRelogin)
        {
            AppendAction(config => config.SaslKerberosMinTimeBeforeRelogin = saslKerberosMinTimeBeforeRelogin);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslUsername(string saslUsername)
        {
            AppendAction(config => config.SaslUsername = saslUsername);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslPassword(string saslPassword)
        {
            AppendAction(config => config.SaslPassword = saslPassword);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerConfig(string saslOauthbearerConfig)
        {
            AppendAction(config => config.SaslOauthbearerConfig = saslOauthbearerConfig);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableSaslOauthbearerUnsecureJwt(bool? enableSaslOauthbearerUnsecureJwt)
        {
            AppendAction(config => config.EnableSaslOauthbearerUnsecureJwt = enableSaslOauthbearerUnsecureJwt);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerMethod(SaslOauthbearerMethod? saslOauthbearerMethod)
        {
            AppendAction(config => config.SaslOauthbearerMethod = saslOauthbearerMethod);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerClientId(string saslOauthbearerClientId)
        {
            AppendAction(config => config.SaslOauthbearerClientId = saslOauthbearerClientId);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerClientSecret(string saslOauthbearerClientSecret)
        {
            AppendAction(config => config.SaslOauthbearerClientSecret = saslOauthbearerClientSecret);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerScope(string saslOauthbearerScope)
        {
            AppendAction(config => config.SaslOauthbearerScope = saslOauthbearerScope);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerExtensions(string saslOauthbearerExtensions)
        {
            AppendAction(config => config.SaslOauthbearerExtensions = saslOauthbearerExtensions);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerTokenEndpointUrl(string saslOauthbearerTokenEndpointUrl)
        {
            AppendAction(config => config.SaslOauthbearerTokenEndpointUrl = saslOauthbearerTokenEndpointUrl);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerGrantType(SaslOauthbearerGrantType? saslOauthbearerGrantType)
        {
            AppendAction(config => config.SaslOauthbearerGrantType = saslOauthbearerGrantType);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionAlgorithm(SaslOauthbearerAssertionAlgorithm? saslOauthbearerAssertionAlgorithm)
        {
            AppendAction(config => config.SaslOauthbearerAssertionAlgorithm = saslOauthbearerAssertionAlgorithm);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionPrivateKeyFile(string saslOauthbearerAssertionPrivateKeyFile)
        {
            AppendAction(config => config.SaslOauthbearerAssertionPrivateKeyFile = saslOauthbearerAssertionPrivateKeyFile);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionPrivateKeyPassphrase(string saslOauthbearerAssertionPrivateKeyPassphrase)
        {
            AppendAction(config => config.SaslOauthbearerAssertionPrivateKeyPassphrase = saslOauthbearerAssertionPrivateKeyPassphrase);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionPrivateKeyPem(string saslOauthbearerAssertionPrivateKeyPem)
        {
            AppendAction(config => config.SaslOauthbearerAssertionPrivateKeyPem = saslOauthbearerAssertionPrivateKeyPem);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionFile(string saslOauthbearerAssertionFile)
        {
            AppendAction(config => config.SaslOauthbearerAssertionFile = saslOauthbearerAssertionFile);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionClaimAud(string saslOauthbearerAssertionClaimAud)
        {
            AppendAction(config => config.SaslOauthbearerAssertionClaimAud = saslOauthbearerAssertionClaimAud);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionClaimExpSeconds(int? saslOauthbearerAssertionClaimExpSeconds)
        {
            AppendAction(config => config.SaslOauthbearerAssertionClaimExpSeconds = saslOauthbearerAssertionClaimExpSeconds);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionClaimIss(string saslOauthbearerAssertionClaimIss)
        {
            AppendAction(config => config.SaslOauthbearerAssertionClaimIss = saslOauthbearerAssertionClaimIss);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionClaimJtiInclude(bool? saslOauthbearerAssertionClaimJtiInclude)
        {
            AppendAction(config => config.SaslOauthbearerAssertionClaimJtiInclude = saslOauthbearerAssertionClaimJtiInclude);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionClaimNbfSeconds(int? saslOauthbearerAssertionClaimNbfSeconds)
        {
            AppendAction(config => config.SaslOauthbearerAssertionClaimNbfSeconds = saslOauthbearerAssertionClaimNbfSeconds);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionClaimSub(string saslOauthbearerAssertionClaimSub)
        {
            AppendAction(config => config.SaslOauthbearerAssertionClaimSub = saslOauthbearerAssertionClaimSub);
            return this;
        }

        public IKafkaProducerConfigBuilder WithSaslOauthbearerAssertionJwtTemplateFile(string saslOauthbearerAssertionJwtTemplateFile)
        {
            AppendAction(config => config.SaslOauthbearerAssertionJwtTemplateFile = saslOauthbearerAssertionJwtTemplateFile);
            return this;
        }

        public IKafkaProducerConfigBuilder WithPluginLibraryPaths(string pluginLibraryPaths)
        {
            AppendAction(config => config.PluginLibraryPaths = pluginLibraryPaths);
            return this;
        }

        public IKafkaProducerConfigBuilder WithClientRack(string clientRack)
        {
            AppendAction(config => config.ClientRack = clientRack);
            return this;
        }

        public IKafkaProducerConfigBuilder WithRetryBackoffMs(int? retryBackoffMs)
        {
            AppendAction(config => config.RetryBackoffMs = retryBackoffMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithRetryBackoffMaxMs(int? retryBackoffMaxMs)
        {
            AppendAction(config => config.RetryBackoffMaxMs = retryBackoffMaxMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithClientDnsLookup(ClientDnsLookup? clientDnsLookup)
        {
            AppendAction(config => config.ClientDnsLookup = clientDnsLookup);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableMetricsPush(bool? enableMetricsPush)
        {
            AppendAction(config => config.EnableMetricsPush = enableMetricsPush);
            return this;
        }

        #endregion IClientConfigBuilder Members

        #region IProducerConfigBuilder Members

        public IKafkaProducerConfigBuilder WithEnableBackgroundPoll(bool? enableBackgroundPoll)
        {
            AppendAction(config => config.EnableBackgroundPoll = enableBackgroundPoll);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableDeliveryReports(bool? enableDeliveryReports)
        {
            AppendAction(config => config.EnableDeliveryReports = enableDeliveryReports);
            return this;
        }

        public IKafkaProducerConfigBuilder WithDeliveryReportFields(string deliveryReportFields)
        {
            AppendAction(config => config.DeliveryReportFields = deliveryReportFields);
            return this;
        }

        public IKafkaProducerConfigBuilder WithRequestTimeoutMs(int? requestTimeoutMs)
        {
            AppendAction(config => config.RequestTimeoutMs = requestTimeoutMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMessageTimeoutMs(int? messageTimeoutMs)
        {
            AppendAction(config => config.MessageTimeoutMs = messageTimeoutMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithPartitioner(Partitioner? partitioner)
        {
            AppendAction(config => config.Partitioner = partitioner);
            return this;
        }

        public IKafkaProducerConfigBuilder WithCompressionLevel(int? compressionLevel)
        {
            AppendAction(config => config.CompressionLevel = compressionLevel);
            return this;
        }

        public IKafkaProducerConfigBuilder WithTransactionalId(string transactionalId)
        {
            AppendAction(config => config.TransactionalId = transactionalId);
            return this;
        }

        public IKafkaProducerConfigBuilder WithTransactionTimeoutMs(int? transactionTimeoutMs)
        {
            AppendAction(config => config.TransactionTimeoutMs = transactionTimeoutMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableIdempotence(bool? enableIdempotence)
        {
            AppendAction(config => config.EnableIdempotence = enableIdempotence);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableGaplessGuarantee(bool? enableGaplessGuarantee)
        {
            AppendAction(config => config.EnableGaplessGuarantee = enableGaplessGuarantee);
            return this;
        }

        public IKafkaProducerConfigBuilder WithQueueBufferingMaxMessages(int? queueBufferingMaxMessages)
        {
            AppendAction(config => config.QueueBufferingMaxMessages = queueBufferingMaxMessages);
            return this;
        }

        public IKafkaProducerConfigBuilder WithQueueBufferingMaxKbytes(int? queueBufferingMaxKbytes)
        {
            AppendAction(config => config.QueueBufferingMaxKbytes = queueBufferingMaxKbytes);
            return this;
        }

        public IKafkaProducerConfigBuilder WithLingerMs(double? lingerMs)
        {
            AppendAction(config => config.LingerMs = lingerMs);
            return this;
        }

        public IKafkaProducerConfigBuilder WithMessageSendMaxRetries(int? messageSendMaxRetries)
        {
            AppendAction(config => config.MessageSendMaxRetries = messageSendMaxRetries);
            return this;
        }

        public IKafkaProducerConfigBuilder WithQueueBufferingBackpressureThreshold(int? queueBufferingBackpressureThreshold)
        {
            AppendAction(config => config.QueueBufferingBackpressureThreshold = queueBufferingBackpressureThreshold);
            return this;
        }

        public IKafkaProducerConfigBuilder WithCompressionType(CompressionType? compressionType)
        {
            AppendAction(config => config.CompressionType = compressionType);
            return this;
        }

        public IKafkaProducerConfigBuilder WithBatchNumMessages(int? batchNumMessages)
        {
            AppendAction(config => config.BatchNumMessages = batchNumMessages);
            return this;
        }

        public IKafkaProducerConfigBuilder WithBatchSize(int? batchSize)
        {
            AppendAction(config => config.BatchSize = batchSize);
            return this;
        }

        public IKafkaProducerConfigBuilder WithStickyPartitioningLingerMs(int? stickyPartitioningLingerMs)
        {
            AppendAction(config => config.StickyPartitioningLingerMs = stickyPartitioningLingerMs);
            return this;
        }

        #endregion IProducerConfigBuilder Members

        #region IKafkaProducerConfigBuilder Members

        public IKafkaProducerConfigBuilder FromConfiguration(string sectionKey)
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

        public IKafkaProducerConfigBuilder WithDefaultTopic(string defaultTopic)
        {
            AppendAction(config => config.DefaultTopic = defaultTopic);
            return this;
        }

        public IKafkaProducerConfigBuilder WithDefaultPartition(Partition defaultPartition)
        {
            AppendAction(config => config.DefaultPartition = defaultPartition);
            return this;
        }

        public IKafkaProducerConfigBuilder WithDefaultTimeout(TimeSpan defaultTimeout)
        {
            AppendAction(config => config.DefaultTimeout = defaultTimeout);
            return this;
        }

        public IKafkaProducerConfigBuilder WithPollAfterProducing(bool pollAfterProducing)
        {
            AppendAction(config => config.PollAfterProducing = pollAfterProducing);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(config => config.EnableLogging = enableLogging);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics)
        {
            AppendAction(config => config.EnableDiagnostics = enableDiagnostics);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure)
        {
            AppendAction(config => config.EnableRetryOnFailure = enableRetryOnFailure);
            return this;
        }

        public IKafkaProducerConfigBuilder WithEnableInterceptorExceptionPropagation(bool enableInterceptorExceptionPropagation)
        {
            AppendAction(config => config.EnableInterceptorExceptionPropagation = enableInterceptorExceptionPropagation);
            return this;
        }

        #endregion IKafkaProducerConfigBuilder Members

        public static IKafkaProducerConfig BuildConfig(
            IConfiguration configuration = null,
            IKafkaProducerConfig producerConfig = null,
            Action<IKafkaProducerConfigBuilder> configureProducer = null)
        {
            using var builder = new KafkaProducerConfigBuilder(producerConfig, configuration);

            configureProducer?.Invoke(builder);

            return builder.Build();
        }
    }
}
