using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal sealed class KafkaServersInfo
    {
        private const char PortSeparator = ':';
        private const string JoinSeparator = ",";
        private static readonly char[] SplitSeparators = new[] { ',' };
        private static readonly ConcurrentDictionary<string, KafkaServersInfo> ServersInfo = new();

        public string ServerHostnames { get; }
        public string ServerIpAddresses { get; }

        private KafkaServersInfo(string bootstrapServers)
        {
            ServerHostnames = GetServerHostnames(bootstrapServers);
            ServerIpAddresses = GetServerIpAddresses(bootstrapServers);
        }

        public static KafkaServersInfo Parse(string bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(bootstrapServers))
            {
                return null;
            }

            var serversInfo = ServersInfo.GetOrAdd(bootstrapServers, new KafkaServersInfo(bootstrapServers));

            return serversInfo;
        }

        private static string GetServerHostnames(string bootstrapServers)
        {
            try
            {
                var serverHostnames = bootstrapServers
                    .Split(SplitSeparators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(bootstrapServer => ExtractServerHostName(bootstrapServer.Trim()))
                    .Where(serverHostname => !string.IsNullOrWhiteSpace(serverHostname));

                return string.Join(JoinSeparator, serverHostnames);
            }
            catch
            {
                return null;
            }
        }

        private static string GetServerIpAddresses(string bootstrapServers)
        {
            try
            {
                var serverIpAddresses = bootstrapServers
                    .Split(SplitSeparators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(bootstrapServer => ExtractServerIpAddress(bootstrapServer.Trim()))
                    .Where(serverIpAddress => !string.IsNullOrWhiteSpace(serverIpAddress));

                return string.Join(JoinSeparator, serverIpAddresses);
            }
            catch
            {
                return null;
            }
        }

        private static string ExtractServerHostName(string bootstrapServer)
        {
            var hostEntry = GetHostEntry(bootstrapServer);

            return hostEntry?.HostName;
        }

        private static string ExtractServerIpAddress(string bootstrapServer)
        {
            var hostEntry = GetHostEntry(bootstrapServer);

            var ipAddresses = hostEntry?.AddressList?.Select(ipAddress => ipAddress.ToString())
                ?? Enumerable.Empty<string>();

            return string.Join(JoinSeparator, ipAddresses);
        }

        private static IPHostEntry GetHostEntry(string bootstrapServer)
        {
            var bootstrapServerSpan = bootstrapServer.AsSpan();

            var delimiterIndex = bootstrapServerSpan.IndexOf(PortSeparator);

            if (delimiterIndex > 0)
            {
                bootstrapServerSpan = bootstrapServerSpan[..delimiterIndex];
            }

            var hostNameOrAddress = bootstrapServerSpan.ToString();

            var hostEntry = Dns.GetHostEntry(hostNameOrAddress);

            return hostEntry;
        }
    }
}
