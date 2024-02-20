using System;
using System.Text;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal static class TopicPartitionExtensions
    {
        public static void ValidateState(this TopicPartition topicPartition)
        {
            if (topicPartition is null)
            {
                throw new ArgumentNullException(nameof(topicPartition), $"{nameof(topicPartition)} cannot be null.");
            }

            const string Message = "Invalid Topic/Partition state: ";

            StringBuilder builder = null;

            if (string.IsNullOrWhiteSpace(topicPartition.Topic))
            {
                builder ??= new StringBuilder(Message);
                builder.AppendLine("- Topic cannot be null or whitespace.");
            }

            if (topicPartition.Partition < Partition.Any)
            {
                builder ??= new StringBuilder(Message);
                builder.AppendLine($"- Partition cannot be lower than {Partition.Any.Value}.");
            }

            if (builder is not null)
            {
                throw new InvalidOperationException(builder.ToString());
            }
        }
    }
}
