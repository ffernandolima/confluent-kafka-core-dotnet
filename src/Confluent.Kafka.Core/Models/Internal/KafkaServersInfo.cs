using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal sealed class KafkaServersInfo
    {
        private const char PortSeparator = ':';
        private static readonly char[] SplitSeparators = [','];
        private static readonly ConcurrentDictionary<string, KafkaServersInfo> ServersInfo = [];

        /// <summary>
        /// The first bootstrap entry, as configured:
        /// server.address and server.port describe a single server, and must not come from a reverse DNS lookup.
        /// </summary>
        public string ServerAddress { get; }
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

            var delimiterIndex = firstServer.LastIndexOf(PortSeparator);

            if (delimiterIndex <= 0)
            {
                ServerAddress = firstServer;
                return;
            }

#if NETSTANDARD2_0
            ServerAddress = firstServer.Substring(0, delimiterIndex);
            var portText = firstServer.Substring(delimiterIndex + 1);
#else
            ServerAddress = firstServer[..delimiterIndex];
            var portText = firstServer[(delimiterIndex + 1)..];
#endif

            if (int.TryParse(portText, out var port))
            {
                ServerPort = port;
            }
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
    }
}
