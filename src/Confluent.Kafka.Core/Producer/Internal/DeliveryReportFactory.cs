namespace Confluent.Kafka.Core.Producer.Internal
{
    internal static class DeliveryReportFactory
    {
        public static DeliveryReport<TKey, TValue> CreateDefault<TKey, TValue>(
            TopicPartition partition,
            Message<TKey, TValue> message)
        {
            var deliveryReport = new DeliveryReport<TKey, TValue>
            {
                Message = message,
                Error = new Error(ErrorCode.NoError),
                Status = PersistenceStatus.PossiblyPersisted,
                TopicPartitionOffset = new TopicPartitionOffset(partition, Offset.Unset)
            };

            return deliveryReport;
        }
    }
}
