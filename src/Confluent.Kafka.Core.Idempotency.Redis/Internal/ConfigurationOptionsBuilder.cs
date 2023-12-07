using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

namespace Confluent.Kafka.Core.Idempotency.Redis.Internal
{
    internal sealed class ConfigurationOptionsBuilder :
        FunctionalBuilder<ConfigurationOptions, ConfigurationOptionsBuilder>,
        IConfigurationOptionsBuilder
    {
        protected override ConfigurationOptions CreateSubject() => new()
        {
            AbortOnConnectFail = false
        };

        public IConfigurationOptionsBuilder WithCertificateSelection(LocalCertificateSelectionCallback certificateSelection)
        {
            AppendAction(options =>
            {
                if (certificateSelection is not null)
                {
                    options.CertificateSelection += certificateSelection;
                }
            });
            return this;
        }

        public IConfigurationOptionsBuilder WithCertificateValidation(RemoteCertificateValidationCallback certificateValidation)
        {
            AppendAction(options =>
            {
                if (certificateValidation is not null)
                {
                    options.CertificateValidation += certificateValidation;
                }
            });
            return this;
        }

        public IConfigurationOptionsBuilder WithDefaults(DefaultOptionsProvider defaults)
        {
            AppendAction(options => options.Defaults = defaults);
            return this;
        }

        public IConfigurationOptionsBuilder WithBeforeSocketConnect(Action<EndPoint, ConnectionType, Socket> beforeSocketConnect)
        {
            AppendAction(options => options.BeforeSocketConnect = beforeSocketConnect);
            return this;
        }

        public IConfigurationOptionsBuilder WithAbortOnConnectFail(bool abortOnConnectFail)
        {
            AppendAction(options => options.AbortOnConnectFail = abortOnConnectFail);
            return this;
        }

        public IConfigurationOptionsBuilder WithAllowAdmin(bool allowAdmin)
        {
            AppendAction(options => options.AllowAdmin = allowAdmin);
            return this;
        }

        public IConfigurationOptionsBuilder WithAsyncTimeout(int asyncTimeout)
        {
            AppendAction(options => options.AsyncTimeout = asyncTimeout);
            return this;
        }

        public IConfigurationOptionsBuilder WithSetClientLibrary(bool setClientLibrary)
        {
            AppendAction(options => options.SetClientLibrary = setClientLibrary);
            return this;
        }

        public IConfigurationOptionsBuilder WithLibraryName(string libraryName)
        {
            AppendAction(options => options.LibraryName = libraryName);
            return this;
        }

        public IConfigurationOptionsBuilder WithChannelPrefix(RedisChannel channelPrefix)
        {
            AppendAction(options => options.ChannelPrefix = channelPrefix);
            return this;
        }

        public IConfigurationOptionsBuilder WithCheckCertificateRevocation(bool checkCertificateRevocation)
        {
            AppendAction(options => options.CheckCertificateRevocation = checkCertificateRevocation);
            return this;
        }

        public IConfigurationOptionsBuilder WithClientName(string clientName)
        {
            AppendAction(options => options.ClientName = clientName);
            return this;
        }

        public IConfigurationOptionsBuilder WithConnectRetry(int connectRetry)
        {
            AppendAction(options => options.ConnectRetry = connectRetry);
            return this;
        }

        public IConfigurationOptionsBuilder WithCommandMap(CommandMap commandMap)
        {
            AppendAction(options => options.CommandMap = commandMap);
            return this;
        }

        public IConfigurationOptionsBuilder WithConfigurationChannel(string configurationChannel)
        {
            AppendAction(options => options.ConfigurationChannel = configurationChannel);
            return this;
        }

        public IConfigurationOptionsBuilder WithConnectTimeout(int connectTimeout)
        {
            AppendAction(options => options.ConnectTimeout = connectTimeout);
            return this;
        }

        public IConfigurationOptionsBuilder WithDefaultDatabase(int? defaultDatabase)
        {
            AppendAction(options => options.DefaultDatabase = defaultDatabase);
            return this;
        }

        public IConfigurationOptionsBuilder WithDefaultVersion(Version defaultVersion)
        {
            AppendAction(options => options.DefaultVersion = defaultVersion);
            return this;
        }

        public IConfigurationOptionsBuilder WithEndPoints(EndPointCollection endPoints)
        {
            AppendAction(options =>
            {
                if (options.EndPoints is not null && endPoints is not null && endPoints.Any())
                {
                    foreach (var endPoint in endPoints.Where(endPoint => endPoint is not null))
                    {
                        if (!options.EndPoints.Contains(endPoint))
                        {
                            options.EndPoints.Add(endPoint);
                        }
                    }
                }
            });
            return this;
        }

        public IConfigurationOptionsBuilder WithHeartbeatInterval(TimeSpan heartbeatInterval)
        {
            AppendAction(options => options.HeartbeatInterval = heartbeatInterval);
            return this;
        }

        public IConfigurationOptionsBuilder WithIncludeDetailInExceptions(bool includeDetailInExceptions)
        {
            AppendAction(options => options.IncludeDetailInExceptions = includeDetailInExceptions);
            return this;
        }

        public IConfigurationOptionsBuilder WithIncludePerformanceCountersInExceptions(bool includePerformanceCountersInExceptions)
        {
            AppendAction(options => options.IncludePerformanceCountersInExceptions = includePerformanceCountersInExceptions);
            return this;
        }

        public IConfigurationOptionsBuilder WithKeepAlive(int keepAlive)
        {
            AppendAction(options => options.KeepAlive = keepAlive);
            return this;
        }

        public IConfigurationOptionsBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            AppendAction(options => options.LoggerFactory = loggerFactory);
            return this;
        }

        public IConfigurationOptionsBuilder WithUser(string user)
        {
            AppendAction(options => options.User = user);
            return this;
        }

        public IConfigurationOptionsBuilder WithPassword(string password)
        {
            AppendAction(options => options.Password = password);
            return this;
        }

        public IConfigurationOptionsBuilder WithProxy(Proxy proxy)
        {
            AppendAction(options => options.Proxy = proxy);
            return this;
        }

        public IConfigurationOptionsBuilder WithReconnectRetryPolicy(IReconnectRetryPolicy reconnectRetryPolicy)
        {
            AppendAction(options => options.ReconnectRetryPolicy = reconnectRetryPolicy);
            return this;
        }

        public IConfigurationOptionsBuilder WithBacklogPolicy(BacklogPolicy backlogPolicy)
        {
            AppendAction(options => options.BacklogPolicy = backlogPolicy);
            return this;
        }

        public IConfigurationOptionsBuilder WithResolveDns(bool resolveDns)
        {
            AppendAction(options => options.ResolveDns = resolveDns);
            return this;
        }

        public IConfigurationOptionsBuilder WithServiceName(string serviceName)
        {
            AppendAction(options => options.ServiceName = serviceName);
            return this;
        }

        public IConfigurationOptionsBuilder WithSocketManager(SocketManager socketManager)
        {
            AppendAction(options => options.SocketManager = socketManager);
            return this;
        }

#if NETCOREAPP3_1_OR_GREATER

        public IConfigurationOptionsBuilder WithSslClientAuthenticationOptions(Func<string, SslClientAuthenticationOptions> sslClientAuthenticationOptions)
        {
            AppendAction(options => options.SslClientAuthenticationOptions = sslClientAuthenticationOptions);
            return this;
        }
#endif

        public IConfigurationOptionsBuilder WithSsl(bool ssl)
        {
            AppendAction(options => options.Ssl = ssl);
            return this;
        }

        public IConfigurationOptionsBuilder WithSslHost(string sslHost)
        {
            AppendAction(options => options.SslHost = sslHost);
            return this;
        }

        public IConfigurationOptionsBuilder WithSslProtocols(SslProtocols? sslProtocols)
        {
            AppendAction(options => options.SslProtocols = sslProtocols);
            return this;
        }

        public IConfigurationOptionsBuilder WithSyncTimeout(int syncTimeout)
        {
            AppendAction(options => options.SyncTimeout = syncTimeout);
            return this;
        }

        public IConfigurationOptionsBuilder WithTieBreaker(string tieBreaker)
        {
            AppendAction(options => options.TieBreaker = tieBreaker);
            return this;
        }

        public IConfigurationOptionsBuilder WithConfigCheckSeconds(int configCheckSeconds)
        {
            AppendAction(options => options.ConfigCheckSeconds = configCheckSeconds);
            return this;
        }

        public IConfigurationOptionsBuilder WithTunnel(Tunnel tunnel)
        {
            AppendAction(options => options.Tunnel = tunnel);
            return this;
        }

        public IConfigurationOptionsBuilder WithProtocol(RedisProtocol? protocol)
        {
            AppendAction(options => options.Protocol = protocol);
            return this;
        }

        internal static ConfigurationOptions Build(Action<IConfigurationOptionsBuilder> configureOptions)
        {
            using var builder = new ConfigurationOptionsBuilder();

            configureOptions?.Invoke(builder);

            var options = builder.Build();

            return options;
        }
    }
}
