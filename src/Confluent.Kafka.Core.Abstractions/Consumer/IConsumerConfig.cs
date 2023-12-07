using Confluent.Kafka.Core.Client;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IConsumerConfig : IClientConfig
    {
        string ConsumeResultFields { get; }

        AutoOffsetReset? AutoOffsetReset { get; }

        string GroupId { get; }

        string GroupInstanceId { get; }

        PartitionAssignmentStrategy? PartitionAssignmentStrategy { get; }

        int? SessionTimeoutMs { get; }

        int? HeartbeatIntervalMs { get; }

        string GroupProtocolType { get; }

        int? CoordinatorQueryIntervalMs { get; }

        int? MaxPollIntervalMs { get; }

        bool? EnableAutoCommit { get; }

        int? AutoCommitIntervalMs { get; }

        bool? EnableAutoOffsetStore { get; }

        int? QueuedMinMessages { get; }

        int? QueuedMaxMessagesKbytes { get; }

        int? FetchWaitMaxMs { get; }

        int? FetchQueueBackoffMs { get; }

        int? MaxPartitionFetchBytes { get; }

        int? FetchMaxBytes { get; }

        int? FetchMinBytes { get; }

        int? FetchErrorBackoffMs { get; }

        IsolationLevel? IsolationLevel { get; }

        bool? EnablePartitionEof { get; }

        bool? CheckCrcs { get; }
    }
}
