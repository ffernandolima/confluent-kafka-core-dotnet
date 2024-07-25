using Confluent.Kafka.Core.Client;

namespace Confluent.Kafka.Core.Producer
{
    public interface IProducerConfigBuilder<TBuilder> :
        IClientConfigBuilder<TBuilder>
        where TBuilder : IProducerConfigBuilder<TBuilder>
    {
        TBuilder WithEnableBackgroundPoll(bool? enableBackgroundPoll);

        TBuilder WithEnableDeliveryReports(bool? enableDeliveryReports);

        TBuilder WithDeliveryReportFields(string deliveryReportFields);

        TBuilder WithRequestTimeoutMs(int? requestTimeoutMs);

        TBuilder WithMessageTimeoutMs(int? messageTimeoutMs);

        TBuilder WithPartitioner(Partitioner? partitioner);

        TBuilder WithCompressionLevel(int? compressionLevel);

        TBuilder WithTransactionalId(string transactionalId);

        TBuilder WithTransactionTimeoutMs(int? transactionTimeoutMs);

        TBuilder WithEnableIdempotence(bool? enableIdempotence);

        TBuilder WithEnableGaplessGuarantee(bool? enableGaplessGuarantee);

        TBuilder WithQueueBufferingMaxMessages(int? queueBufferingMaxMessages);

        TBuilder WithQueueBufferingMaxKbytes(int? queueBufferingMaxKbytes);

        TBuilder WithLingerMs(double? lingerMs);

        TBuilder WithMessageSendMaxRetries(int? messageSendMaxRetries);

        TBuilder WithQueueBufferingBackpressureThreshold(int? queueBufferingBackpressureThreshold);

        TBuilder WithCompressionType(CompressionType? compressionType);

        TBuilder WithBatchNumMessages(int? batchNumMessages);

        TBuilder WithBatchSize(int? batchSize);

        TBuilder WithStickyPartitioningLingerMs(int? stickyPartitioningLingerMs);
    }
}