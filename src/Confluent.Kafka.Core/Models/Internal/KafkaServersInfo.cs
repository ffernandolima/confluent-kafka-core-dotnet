using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal sealed class KafkaServersInfo
    {
        private const char PortSeparator = ':';
        private static readonly char[] SplitSeparators = [','];
        private static readonly ConcurrentDictionary<string, KafkaServersInfo> ServersInfo = [];

        /// <summary>
        /// server.address and server.port describe a single server, so only the first bootstrap entry is described. 
        /// Joining every host into one tag produced a value no backend can use.
        /// </summary>
        public string ServerHostname { get; }
        public string ServerIpAddress { get; }
        public int? ServerPort { get; }

        private KafkaServersInfo(string bootstrapServers)
        {
            var firstServer = bootstrapServers
                .Split(SplitSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(bootstrapServer => bootstrapServer.Trim())
                .FirstOrDefault(bootstrapServer => !string.IsNullOrWhiteSpace(bootstrapServer));

            if (firstServer is null)
            {
                return;
            }

            var (hostNameOrAddress, port) = SplitHostAndPort(firstServer);

            ServerPort = port;
            ServerHostname = GetServerHostname(hostNameOrAddress);
            ServerIpAddress = GetServerIpAddress(hostNameOrAddress);
        }

        public static KafkaServersInfo Parse(string bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(bootstrapServers))
            {
                return null;
            }

            var serversInfo = ServersInfo.GetOrAdd(bootstrapServers, static key => new KafkaServersInfo(key));

            return serversInfo;
        }

        private static (string HostNameOrAddress, int? Port) SplitHostAndPort(string bootstrapServer)
        {
            var delimiterIndex = bootstrapServer.LastIndexOf(PortSeparator);

            if (delimiterIndex <= 0)
            {
                return (bootstrapServer, null);
            }

#if NETSTANDARD2_0
            var hostNameOrAddress = bootstrapServer.Substring(0, delimiterIndex);
            var portText = bootstrapServer.Substring(delimiterIndex + 1);
#else
            var hostNameOrAddress = bootstrapServer[..delimiterIndex];
            var portText = bootstrapServer[(delimiterIndex + 1)..];
#endif

            return (hostNameOrAddress, int.TryParse(portText, out var port) ? port : null);
        }

        private static string GetServerHostname(string hostNameOrAddress)
        {
            try
            {
                return GetHostEntry(hostNameOrAddress)?.HostName;
            }
            catch
            {
                return null;
            }
        }

        private static string GetServerIpAddress(string hostNameOrAddress)
        {
            try
            {
                var ipAddress = GetHostEntry(hostNameOrAddress)?.AddressList?.FirstOrDefault();

                return ipAddress?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static IPHostEntry GetHostEntry(string hostNameOrAddress) => Dns.GetHostEntry(hostNameOrAddress);
    }
}
