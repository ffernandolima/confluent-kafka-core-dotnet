using Confluent.Kafka.Core.Client;

namespace Confluent.Kafka.Core.Consumer
{
    public interface IConsumerConfigBuilder<TBuilder> :
        IClientConfigBuilder<TBuilder>
        where TBuilder : IConsumerConfigBuilder<TBuilder>
    {
        TBuilder WithConsumeResultFields(string consumeResultFields);

        TBuilder WithAutoOffsetReset(AutoOffsetReset? autoOffsetReset);

        TBuilder WithGroupId(string groupId);

        TBuilder WithGroupInstanceId(string groupInstanceId);

        TBuilder WithPartitionAssignmentStrategy(PartitionAssignmentStrategy? partitionAssignmentStrategy);

        TBuilder WithSessionTimeoutMs(int? sessionTimeoutMs);

        TBuilder WithHeartbeatIntervalMs(int? heartbeatIntervalMs);

        TBuilder WithGroupProtocolType(string groupProtocolType);

        TBuilder WithCoordinatorQueryIntervalMs(int? coordinatorQueryIntervalMs);

        TBuilder WithMaxPollIntervalMs(int? maxPollIntervalMs);

        TBuilder WithEnableAutoCommit(bool? enableAutoCommit);

        TBuilder WithAutoCommitIntervalMs(int? autoCommitIntervalMs);

        TBuilder WithEnableAutoOffsetStore(bool? enableAutoOffsetStore);

        TBuilder WithQueuedMinMessages(int? queuedMinMessages);

        TBuilder WithQueuedMaxMessagesKbytes(int? queuedMaxMessagesKbytes);

        TBuilder WithFetchWaitMaxMs(int? fetchWaitMaxMs);

        TBuilder WithMaxPartitionFetchBytes(int? maxPartitionFetchBytes);

        TBuilder WithFetchMaxBytes(int? fetchMaxBytes);

        TBuilder WithFetchMinBytes(int? fetchMinBytes);

        TBuilder WithFetchErrorBackoffMs(int? fetchErrorBackoffMs);

        TBuilder WithIsolationLevel(IsolationLevel? isolationLevel);

        TBuilder WithEnablePartitionEof(bool? enablePartitionEof);

        TBuilder WithCheckCrcs(bool? checkCrcs);
    }
}
