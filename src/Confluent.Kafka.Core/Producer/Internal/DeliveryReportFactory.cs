using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class DeliveryReportFactory
    {
        private static readonly Lazy<DeliveryReportFactory> Factory = new(
            () => new DeliveryReportFactory(), isThreadSafe: true);

        public static DeliveryReportFactory Instance => Factory.Value;

        private DeliveryReportFactory()
        { }

        public DeliveryReport<TKey, TValue> CreateDefault<TKey, TValue>(
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
