using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducer<TKey, TValue> : IProducer<TKey, TValue>
    {
        IKafkaProducerOptions<TKey, TValue> Options { get; }

        void Produce(
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);

        void Produce(
            Partition partition,
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);

        void Produce(
            string topic,
            Partition partition,
            Message<TKey, TValue> message,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);

        Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            Message<TKey, TValue> message,
            CancellationToken cancellationToken = default);

        Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            Partition partition,
            Message<TKey, TValue> message,
            CancellationToken cancellationToken = default);

        Task<DeliveryResult<TKey, TValue>> ProduceAsync(
            string topic,
            Partition partition,
            Message<TKey, TValue> message,
            CancellationToken cancellationToken = default);
    }
}
