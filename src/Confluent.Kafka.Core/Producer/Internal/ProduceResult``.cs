using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class ProduceResult<TKey, TValue>
    {
        public bool DeliveryHandled { get; private set; }
        public DeliveryReport<TKey, TValue> DeliveryReport { get; private set; }
        public bool Faulted => DeliveryReport.Error!.IsError;

        public ProduceResult(TopicPartition partition, Message<TKey, TValue> message)
        {
            DeliveryReport = DeliveryReportFactory.Instance.CreateDefault(partition, message);
        }

        public void Complete(DeliveryReport<TKey, TValue> deliveryReport)
        {
            if (deliveryReport is null)
            {
                throw new ArgumentNullException(nameof(deliveryReport), $"{nameof(deliveryReport)} cannot be null.");
            }

            DeliveryHandled = true;
            DeliveryReport = deliveryReport;
        }
    }
}
