using Confluent.Kafka.Core.Client;

namespace Confluent.Kafka.Core.Producer
{
    public interface IProducerConfig : IClientConfig
    {
        bool? EnableBackgroundPoll { get; }

        bool? EnableDeliveryReports { get; }

        string DeliveryReportFields { get; }

        int? RequestTimeoutMs { get; }

        int? MessageTimeoutMs { get; }

        Partitioner? Partitioner { get; }

        int? CompressionLevel { get; }

        string TransactionalId { get; }

        int? TransactionTimeoutMs { get; }

        bool? EnableIdempotence { get; }

        bool? EnableGaplessGuarantee { get; }

        int? QueueBufferingMaxMessages { get; }

        int? QueueBufferingMaxKbytes { get; }

        double? LingerMs { get; }

        int? MessageSendMaxRetries { get; }

        int? RetryBackoffMs { get; }

        int? RetryBackoffMaxMs { get; }

        int? QueueBufferingBackpressureThreshold { get; }

        CompressionType? CompressionType { get; }

        int? BatchNumMessages { get; }

        int? BatchSize { get; }

        int? StickyPartitioningLingerMs { get; }
    }
}
