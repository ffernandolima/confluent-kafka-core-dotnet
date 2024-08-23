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
                if (_id != Guid.Empty)
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
        public byte[] SourceValue { get; init; }
        public string SourceKeyType { get; init; }
        public string SourceValueType { get; init; }
        public ErrorCode ErrorCode { get; init; }
        public string Reason { get; init; }

        public KafkaMetadataMessage Clone() => new()
        {
            Id = Guid.NewGuid(),
            SourceId = SourceId,
            SourceTopic = SourceTopic,
            SourceGroupId = SourceGroupId,
            SourcePartition = SourcePartition,
            SourceOffset = SourceOffset,
            SourceKey = SourceKey,
            SourceValue = SourceValue,
            SourceKeyType = SourceKeyType,
            SourceValueType = SourceValueType,
            ErrorCode = ErrorCode,
            Reason = Reason
        };
    }
}
