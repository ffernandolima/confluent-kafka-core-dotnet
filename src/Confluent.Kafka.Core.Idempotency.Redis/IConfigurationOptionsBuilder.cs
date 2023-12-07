using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Configuration;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public interface IConfigurationOptionsBuilder
    {
        IConfigurationOptionsBuilder WithCertificateSelection(LocalCertificateSelectionCallback certificateSelection);

        IConfigurationOptionsBuilder WithCertificateValidation(RemoteCertificateValidationCallback certificateValidation);

        IConfigurationOptionsBuilder WithDefaults(DefaultOptionsProvider defaults);

        IConfigurationOptionsBuilder WithBeforeSocketConnect(Action<EndPoint, ConnectionType, Socket> beforeSocketConnect);

        IConfigurationOptionsBuilder WithAbortOnConnectFail(bool abortOnConnectFail);

        IConfigurationOptionsBuilder WithAllowAdmin(bool allowAdmin);

        IConfigurationOptionsBuilder WithAsyncTimeout(int asyncTimeout);

        IConfigurationOptionsBuilder WithSetClientLibrary(bool setClientLibrary);

        IConfigurationOptionsBuilder WithLibraryName(string libraryName);

        IConfigurationOptionsBuilder WithChannelPrefix(RedisChannel channelPrefix);

        IConfigurationOptionsBuilder WithCheckCertificateRevocation(bool checkCertificateRevocation);

        IConfigurationOptionsBuilder WithClientName(string clientName);

        IConfigurationOptionsBuilder WithConnectRetry(int connectRetry);

        IConfigurationOptionsBuilder WithCommandMap(CommandMap commandMap);

        IConfigurationOptionsBuilder WithConfigurationChannel(string configurationChannel);

        IConfigurationOptionsBuilder WithConnectTimeout(int connectTimeout);

        IConfigurationOptionsBuilder WithDefaultDatabase(int? defaultDatabase);

        IConfigurationOptionsBuilder WithDefaultVersion(Version defaultVersion);

        IConfigurationOptionsBuilder WithEndPoints(EndPointCollection endPoints);

        IConfigurationOptionsBuilder WithHeartbeatInterval(TimeSpan heartbeatInterval);

        IConfigurationOptionsBuilder WithIncludeDetailInExceptions(bool includeDetailInExceptions);

        IConfigurationOptionsBuilder WithIncludePerformanceCountersInExceptions(bool includePerformanceCountersInExceptions);

        IConfigurationOptionsBuilder WithKeepAlive(int keepAlive);

        IConfigurationOptionsBuilder WithLoggerFactory(ILoggerFactory loggerFactory);

        IConfigurationOptionsBuilder WithUser(string user);

        IConfigurationOptionsBuilder WithPassword(string password);

        IConfigurationOptionsBuilder WithProxy(Proxy proxy);

        IConfigurationOptionsBuilder WithReconnectRetryPolicy(IReconnectRetryPolicy reconnectRetryPolicy);

        IConfigurationOptionsBuilder WithBacklogPolicy(BacklogPolicy backlogPolicy);

        IConfigurationOptionsBuilder WithResolveDns(bool resolveDns);

        IConfigurationOptionsBuilder WithServiceName(string serviceName);

        IConfigurationOptionsBuilder WithSocketManager(SocketManager socketManager);

#if NETCOREAPP3_1_OR_GREATER

        IConfigurationOptionsBuilder WithSslClientAuthenticationOptions(Func<string, SslClientAuthenticationOptions> sslClientAuthenticationOptions);
#endif

        IConfigurationOptionsBuilder WithSsl(bool ssl);

        IConfigurationOptionsBuilder WithSslHost(string sslHost);

        IConfigurationOptionsBuilder WithSslProtocols(SslProtocols? sslProtocols);

        IConfigurationOptionsBuilder WithSyncTimeout(int syncTimeout);

        IConfigurationOptionsBuilder WithTieBreaker(string tieBreaker);

        IConfigurationOptionsBuilder WithConfigCheckSeconds(int configCheckSeconds);

        IConfigurationOptionsBuilder WithTunnel(Tunnel tunnel);

        IConfigurationOptionsBuilder WithProtocol(RedisProtocol? protocol);
    }
}
