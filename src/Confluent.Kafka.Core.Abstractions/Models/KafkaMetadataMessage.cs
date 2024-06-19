using System;

namespace Confluent.Kafka.Core.Models
{
    public class KafkaMetadataMessage : IMessageValue
    {
        private Guid _id;
        public Guid Id
        {
            get => _id;
            set
            {
                if (!_id.Equals(Guid.Empty))
                {
                    throw new InvalidOperationException("Id has already been assigned.");
                }
                _id = value;
            }
        }

        public object SourceId { get; init; }
        public string SourceTopic { get; init; }
        public string SourceGroupId { get; init; }
        public Partition SourcePartition { get; init; }
        public Offset SourceOffset { get; init; }
        public byte[] SourceKey { get; init; }
        public byte[] SourceMessage { get; init; }
        public string SourceKeyType { get; init; }
        public string SourceMessageType { get; init; }
        public ErrorCode ErrorCode { get; init; }
        public string Reason { get; init; }
    }
}
