using Confluent.Kafka.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerConfigBuilder :
        FunctionalBuilder<KafkaConsumerConfig, IKafkaConsumerConfig, KafkaConsumerConfigBuilder>,
        IKafkaConsumerConfigBuilder
    {
        protected override KafkaConsumerConfig CreateSubject() => new()
        {
            GroupId = $"{Guid.NewGuid()}"
        };

        public KafkaConsumerConfigBuilder(IKafkaConsumerConfig seedSubjectAbs = null)
            : base(seedSubjectAbs)
        { }

        #region IConfigBuilder Members

        public IKafkaConsumerConfigBuilder WithCancellationDelayMaxMs(int cancellationDelayMaxMs)
        {
            AppendAction(config => config.CancellationDelayMaxMs = cancellationDelayMaxMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithConfigProperty(KeyValuePair<string, string> configProperty)
        {
            AppendAction(config => config.Set(configProperty.Key, configProperty.Value));
            return this;
        }

        public IKafkaConsumerConfigBuilder WithConfigProperty(string configPropertyKey, string configPropertyValue)
        {
            AppendAction(config => config.Set(configPropertyKey, configPropertyValue));
            return this;
        }

        #endregion IConfigBuilder Members

        #region IClientConfigBuilder Members

        public IKafkaConsumerConfigBuilder WithSaslMechanism(SaslMechanism? saslMechanism)
        {
            AppendAction(config => config.SaslMechanism = saslMechanism);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithAcks(Acks? acks)
        {
            AppendAction(config => config.Acks = acks);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithClientId(string clientId)
        {
            AppendAction(config => config.ClientId = clientId);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithBootstrapServers(string bootstrapServers)
        {
            AppendAction(config => config.BootstrapServers = bootstrapServers);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithMessageMaxBytes(int? messageMaxBytes)
        {
            AppendAction(config => config.MessageMaxBytes = messageMaxBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithMessageCopyMaxBytes(int? messageCopyMaxBytes)
        {
            AppendAction(config => config.MessageCopyMaxBytes = messageCopyMaxBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithReceiveMessageMaxBytes(int? receiveMessageMaxBytes)
        {
            AppendAction(config => config.ReceiveMessageMaxBytes = receiveMessageMaxBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithMaxInFlight(int? maxInFlight)
        {
            AppendAction(config => config.MaxInFlight = maxInFlight);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithTopicMetadataRefreshIntervalMs(int? topicMetadataRefreshIntervalMs)
        {
            AppendAction(config => config.TopicMetadataRefreshIntervalMs = topicMetadataRefreshIntervalMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithMetadataMaxAgeMs(int? metadataMaxAgeMs)
        {
            AppendAction(config => config.MetadataMaxAgeMs = metadataMaxAgeMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithTopicMetadataRefreshFastIntervalMs(int? topicMetadataRefreshFastIntervalMs)
        {
            AppendAction(config => config.TopicMetadataRefreshFastIntervalMs = topicMetadataRefreshFastIntervalMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithTopicMetadataRefreshSparse(bool? topicMetadataRefreshSparse)
        {
            AppendAction(config => config.TopicMetadataRefreshSparse = topicMetadataRefreshSparse);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithTopicMetadataPropagationMaxMs(int? topicMetadataPropagationMaxMs)
        {
            AppendAction(config => config.TopicMetadataPropagationMaxMs = topicMetadataPropagationMaxMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithTopicBlacklist(string topicBlacklist)
        {
            AppendAction(config => config.TopicBlacklist = topicBlacklist);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithDebug(string debug)
        {
            AppendAction(config => config.Debug = debug);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSocketTimeoutMs(int? socketTimeoutMs)
        {
            AppendAction(config => config.SocketTimeoutMs = socketTimeoutMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSocketSendBufferBytes(int? socketSendBufferBytes)
        {
            AppendAction(config => config.SocketSendBufferBytes = socketSendBufferBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSocketReceiveBufferBytes(int? socketReceiveBufferBytes)
        {
            AppendAction(config => config.SocketReceiveBufferBytes = socketReceiveBufferBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSocketKeepaliveEnable(bool? socketKeepaliveEnable)
        {
            AppendAction(config => config.SocketKeepaliveEnable = socketKeepaliveEnable);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSocketNagleDisable(bool? socketNagleDisable)
        {
            AppendAction(config => config.SocketNagleDisable = socketNagleDisable);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSocketMaxFails(int? socketMaxFails)
        {
            AppendAction(config => config.SocketMaxFails = socketMaxFails);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithBrokerAddressTtl(int? brokerAddressTtl)
        {
            AppendAction(config => config.BrokerAddressTtl = brokerAddressTtl);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithBrokerAddressFamily(BrokerAddressFamily? brokerAddressFamily)
        {
            AppendAction(config => config.BrokerAddressFamily = brokerAddressFamily);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSocketConnectionSetupTimeoutMs(int? socketConnectionSetupTimeoutMs)
        {
            AppendAction(config => config.SocketConnectionSetupTimeoutMs = socketConnectionSetupTimeoutMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithConnectionsMaxIdleMs(int? connectionsMaxIdleMs)
        {
            AppendAction(config => config.ConnectionsMaxIdleMs = connectionsMaxIdleMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithReconnectBackoffMs(int? reconnectBackoffMs)
        {
            AppendAction(config => config.ReconnectBackoffMs = reconnectBackoffMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithReconnectBackoffMaxMs(int? reconnectBackoffMaxMs)
        {
            AppendAction(config => config.ReconnectBackoffMaxMs = reconnectBackoffMaxMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithStatisticsIntervalMs(int? statisticsIntervalMs)
        {
            AppendAction(config => config.StatisticsIntervalMs = statisticsIntervalMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithLogQueue(bool? logQueue)
        {
            AppendAction(config => config.LogQueue = logQueue);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithLogThreadName(bool? logThreadName)
        {
            AppendAction(config => config.LogThreadName = logThreadName);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableRandomSeed(bool? enableRandomSeed)
        {
            AppendAction(config => config.EnableRandomSeed = enableRandomSeed);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithLogConnectionClose(bool? logConnectionClose)
        {
            AppendAction(config => config.LogConnectionClose = logConnectionClose);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithInternalTerminationSignal(int? internalTerminationSignal)
        {
            AppendAction(config => config.InternalTerminationSignal = internalTerminationSignal);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithApiVersionRequest(bool? apiVersionRequest)
        {
            AppendAction(config => config.ApiVersionRequest = apiVersionRequest);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithApiVersionRequestTimeoutMs(int? apiVersionRequestTimeoutMs)
        {
            AppendAction(config => config.ApiVersionRequestTimeoutMs = apiVersionRequestTimeoutMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithApiVersionFallbackMs(int? apiVersionFallbackMs)
        {
            AppendAction(config => config.ApiVersionFallbackMs = apiVersionFallbackMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithBrokerVersionFallback(string brokerVersionFallback)
        {
            AppendAction(config => config.BrokerVersionFallback = brokerVersionFallback);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithAllowAutoCreateTopics(bool? allowAutoCreateTopics)
        {
            AppendAction(config => config.AllowAutoCreateTopics = allowAutoCreateTopics);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSecurityProtocol(SecurityProtocol? securityProtocol)
        {
            AppendAction(config => config.SecurityProtocol = securityProtocol);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCipherSuites(string sslCipherSuites)
        {
            AppendAction(config => config.SslCipherSuites = sslCipherSuites);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCurvesList(string sslCurvesList)
        {
            AppendAction(config => config.SslCurvesList = sslCurvesList);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslSigalgsList(string sslSigalgsList)
        {
            AppendAction(config => config.SslSigalgsList = sslSigalgsList);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslKeyLocation(string sslKeyLocation)
        {
            AppendAction(config => config.SslKeyLocation = sslKeyLocation);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslKeyPassword(string sslKeyPassword)
        {
            AppendAction(config => config.SslKeyPassword = sslKeyPassword);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslKeyPem(string sslKeyPem)
        {
            AppendAction(config => config.SslKeyPem = sslKeyPem);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCertificateLocation(string sslCertificateLocation)
        {
            AppendAction(config => config.SslCertificateLocation = sslCertificateLocation);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCertificatePem(string sslCertificatePem)
        {
            AppendAction(config => config.SslCertificatePem = sslCertificatePem);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCaLocation(string sslCaLocation)
        {
            AppendAction(config => config.SslCaLocation = sslCaLocation);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCaPem(string sslCaPem)
        {
            AppendAction(config => config.SslCaPem = sslCaPem);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCaCertificateStores(string sslCaCertificateStores)
        {
            AppendAction(config => config.SslCaCertificateStores = sslCaCertificateStores);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslCrlLocation(string sslCrlLocation)
        {
            AppendAction(config => config.SslCrlLocation = sslCrlLocation);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslKeystoreLocation(string sslKeystoreLocation)
        {
            AppendAction(config => config.SslKeystoreLocation = sslKeystoreLocation);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslKeystorePassword(string sslKeystorePassword)
        {
            AppendAction(config => config.SslKeystorePassword = sslKeystorePassword);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslProviders(string sslProviders)
        {
            AppendAction(config => config.SslProviders = sslProviders);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslEngineLocation(string sslEngineLocation)
        {
            AppendAction(config => config.SslEngineLocation = sslEngineLocation);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslEngineId(string sslEngineId)
        {
            AppendAction(config => config.SslEngineId = sslEngineId);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableSslCertificateVerification(bool? enableSslCertificateVerification)
        {
            AppendAction(config => config.EnableSslCertificateVerification = enableSslCertificateVerification);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSslEndpointIdentificationAlgorithm(SslEndpointIdentificationAlgorithm? sslEndpointIdentificationAlgorithm)
        {
            AppendAction(config => config.SslEndpointIdentificationAlgorithm = sslEndpointIdentificationAlgorithm);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslKerberosServiceName(string saslKerberosServiceName)
        {
            AppendAction(config => config.SaslKerberosServiceName = saslKerberosServiceName);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslKerberosPrincipal(string saslKerberosPrincipal)
        {
            AppendAction(config => config.SaslKerberosPrincipal = saslKerberosPrincipal);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslKerberosKinitCmd(string saslKerberosKinitCmd)
        {
            AppendAction(config => config.SaslKerberosKinitCmd = saslKerberosKinitCmd);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslKerberosKeytab(string saslKerberosKeytab)
        {
            AppendAction(config => config.SaslKerberosKeytab = saslKerberosKeytab);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslKerberosMinTimeBeforeRelogin(int? saslKerberosMinTimeBeforeRelogin)
        {
            AppendAction(config => config.SaslKerberosMinTimeBeforeRelogin = saslKerberosMinTimeBeforeRelogin);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslUsername(string saslUsername)
        {
            AppendAction(config => config.SaslUsername = saslUsername);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslPassword(string saslPassword)
        {
            AppendAction(config => config.SaslPassword = saslPassword);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslOauthbearerConfig(string saslOauthbearerConfig)
        {
            AppendAction(config => config.SaslOauthbearerConfig = saslOauthbearerConfig);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableSaslOauthbearerUnsecureJwt(bool? enableSaslOauthbearerUnsecureJwt)
        {
            AppendAction(config => config.EnableSaslOauthbearerUnsecureJwt = enableSaslOauthbearerUnsecureJwt);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslOauthbearerMethod(SaslOauthbearerMethod? saslOauthbearerMethod)
        {
            AppendAction(config => config.SaslOauthbearerMethod = saslOauthbearerMethod);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslOauthbearerClientId(string saslOauthbearerClientId)
        {
            AppendAction(config => config.SaslOauthbearerClientId = saslOauthbearerClientId);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslOauthbearerClientSecret(string saslOauthbearerClientSecret)
        {
            AppendAction(config => config.SaslOauthbearerClientSecret = saslOauthbearerClientSecret);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslOauthbearerScope(string saslOauthbearerScope)
        {
            AppendAction(config => config.SaslOauthbearerScope = saslOauthbearerScope);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslOauthbearerExtensions(string saslOauthbearerExtensions)
        {
            AppendAction(config => config.SaslOauthbearerExtensions = saslOauthbearerExtensions);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSaslOauthbearerTokenEndpointUrl(string saslOauthbearerTokenEndpointUrl)
        {
            AppendAction(config => config.SaslOauthbearerTokenEndpointUrl = saslOauthbearerTokenEndpointUrl);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithPluginLibraryPaths(string pluginLibraryPaths)
        {
            AppendAction(config => config.PluginLibraryPaths = pluginLibraryPaths);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithClientRack(string clientRack)
        {
            AppendAction(config => config.ClientRack = clientRack);
            return this;
        }

        #endregion IClientConfigBuilder Members

        #region IConsumerConfigBuilder Members

        public IKafkaConsumerConfigBuilder WithConsumeResultFields(string consumeResultFields)
        {
            AppendAction(config => config.ConsumeResultFields = consumeResultFields);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithAutoOffsetReset(AutoOffsetReset? autoOffsetReset)
        {
            AppendAction(config => config.AutoOffsetReset = autoOffsetReset);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithGroupId(string groupId)
        {
            AppendAction(config => config.GroupId = groupId);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithGroupInstanceId(string groupInstanceId)
        {
            AppendAction(config => config.GroupInstanceId = groupInstanceId);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithPartitionAssignmentStrategy(PartitionAssignmentStrategy? partitionAssignmentStrategy)
        {
            AppendAction(config => config.PartitionAssignmentStrategy = partitionAssignmentStrategy);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithSessionTimeoutMs(int? sessionTimeoutMs)
        {
            AppendAction(config => config.SessionTimeoutMs = sessionTimeoutMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithHeartbeatIntervalMs(int? heartbeatIntervalMs)
        {
            AppendAction(config => config.HeartbeatIntervalMs = heartbeatIntervalMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithGroupProtocolType(string groupProtocolType)
        {
            AppendAction(config => config.GroupProtocolType = groupProtocolType);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithGroupProtocol(GroupProtocol? groupProtocol)
        {
            AppendAction(config => config.GroupProtocol = groupProtocol);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithGroupRemoteAssignor(string groupRemoteAssignor)
        {
            AppendAction(config => config.GroupRemoteAssignor = groupRemoteAssignor);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithCoordinatorQueryIntervalMs(int? coordinatorQueryIntervalMs)
        {
            AppendAction(config => config.CoordinatorQueryIntervalMs = coordinatorQueryIntervalMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithMaxPollIntervalMs(int? maxPollIntervalMs)
        {
            AppendAction(config => config.MaxPollIntervalMs = maxPollIntervalMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableAutoCommit(bool? enableAutoCommit)
        {
            AppendAction(config => config.EnableAutoCommit = enableAutoCommit);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithAutoCommitIntervalMs(int? autoCommitIntervalMs)
        {
            AppendAction(config => config.AutoCommitIntervalMs = autoCommitIntervalMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableAutoOffsetStore(bool? enableAutoOffsetStore)
        {
            AppendAction(config => config.EnableAutoOffsetStore = enableAutoOffsetStore);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithQueuedMinMessages(int? queuedMinMessages)
        {
            AppendAction(config => config.QueuedMinMessages = queuedMinMessages);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithQueuedMaxMessagesKbytes(int? queuedMaxMessagesKbytes)
        {
            AppendAction(config => config.QueuedMaxMessagesKbytes = queuedMaxMessagesKbytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithFetchWaitMaxMs(int? fetchWaitMaxMs)
        {
            AppendAction(config => config.FetchWaitMaxMs = fetchWaitMaxMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithMaxPartitionFetchBytes(int? maxPartitionFetchBytes)
        {
            AppendAction(config => config.MaxPartitionFetchBytes = maxPartitionFetchBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithFetchMaxBytes(int? fetchMaxBytes)
        {
            AppendAction(config => config.FetchMaxBytes = fetchMaxBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithFetchMinBytes(int? fetchMinBytes)
        {
            AppendAction(config => config.FetchMinBytes = fetchMinBytes);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithFetchErrorBackoffMs(int? fetchErrorBackoffMs)
        {
            AppendAction(config => config.FetchErrorBackoffMs = fetchErrorBackoffMs);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithIsolationLevel(IsolationLevel? isolationLevel)
        {
            AppendAction(config => config.IsolationLevel = isolationLevel);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnablePartitionEof(bool? enablePartitionEof)
        {
            AppendAction(config => config.EnablePartitionEof = enablePartitionEof);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithCheckCrcs(bool? checkCrcs)
        {
            AppendAction(config => config.CheckCrcs = checkCrcs);
            return this;
        }

        #endregion IConsumerConfigBuilder Members

        #region IKafkaConsumerConfigBuilder Members

        public IKafkaConsumerConfigBuilder WithTopicSubscriptions(IEnumerable<string> topicSubscriptions)
        {
            AppendAction(config =>
            {
                if (topicSubscriptions is not null && topicSubscriptions.Any(topic => !string.IsNullOrWhiteSpace(topic)))
                {
                    config.TopicSubscriptions = (config.TopicSubscriptions ?? [])
                        .Union(topicSubscriptions.Where(topic => !string.IsNullOrWhiteSpace(topic)));
                }
            });
            return this;
        }

        public IKafkaConsumerConfigBuilder WithPartitionAssignments(IEnumerable<TopicPartition> partitionAssignments)
        {
            AppendAction(config =>
            {
                if (partitionAssignments is not null && partitionAssignments.Any(partition => partition is not null))
                {
                    config.PartitionAssignments = (config.PartitionAssignments ?? [])
                        .Union(partitionAssignments.Where(partition => partition is not null));
                }
            });
            return this;
        }

        public IKafkaConsumerConfigBuilder WithCommitAfterConsuming(bool commitAfterConsuming)
        {
            AppendAction(config => config.CommitAfterConsuming = commitAfterConsuming);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithDefaultTimeout(TimeSpan defaultTimeout)
        {
            AppendAction(config => config.DefaultTimeout = defaultTimeout);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithDefaultBatchSize(int defaultBatchSize)
        {
            AppendAction(config => config.DefaultBatchSize = defaultBatchSize);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(config => config.EnableLogging = enableLogging);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableDiagnostics(bool enableDiagnostics)
        {
            AppendAction(config => config.EnableDiagnostics = enableDiagnostics);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableDeadLetterTopic(bool enableDeadLetterTopic)
        {
            AppendAction(config => config.EnableDeadLetterTopic = enableDeadLetterTopic);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableRetryOnFailure(bool enableRetryOnFailure)
        {
            AppendAction(config => config.EnableRetryOnFailure = enableRetryOnFailure);
            return this;
        }

        public IKafkaConsumerConfigBuilder WithEnableInterceptorExceptionPropagation(bool enableInterceptorExceptionPropagation)
        {
            AppendAction(config => config.EnableInterceptorExceptionPropagation = enableInterceptorExceptionPropagation);
            return this;
        }

        #endregion IKafkaConsumerConfigBuilder Members
    }
}
