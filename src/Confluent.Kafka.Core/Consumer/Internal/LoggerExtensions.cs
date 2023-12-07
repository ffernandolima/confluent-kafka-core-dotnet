using Microsoft.Extensions.Logging;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Debug, Message = "Seeking Topic '{Topic}', Partition {Partition} to Offset {Offset}.")]
        public static partial void LogSeekingTopicPartitionOffset(this ILogger logger, string topic, int partition, long offset);
    }
}
