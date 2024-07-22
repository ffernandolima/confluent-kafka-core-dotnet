using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Producer;
using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics
{
    public interface IKafkaActivityEnricher
    {
        void Enrich(Activity activity, ConsumeException consumeException, IKafkaConsumerConfig consumerConfig);

        void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, ProduceException<TKey, TValue> produceException, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, DeliveryReport<TKey, TValue> deliveryReport, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, DeliveryResult<TKey, TValue> deliveryResult, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, Exception exception, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerWorkerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerWorkerOptions<TKey, TValue> options);
    }
}
