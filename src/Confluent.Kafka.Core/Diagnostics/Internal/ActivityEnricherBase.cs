using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Producer;
using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal abstract class ActivityEnricherBase
    {
        public abstract void Enrich(Activity activity, ConsumeException consumeException, IConsumerConfig consumerConfig);

        public abstract void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerOptions<TKey, TValue> options);

        public abstract void Enrich<TKey, TValue>(Activity activity, ProduceException<TKey, TValue> produceException, IKafkaProducerOptions<TKey, TValue> options);

        public abstract void Enrich<TKey, TValue>(Activity activity, DeliveryReport<TKey, TValue> deliveryReport, IKafkaProducerOptions<TKey, TValue> options);

        public abstract void Enrich<TKey, TValue>(Activity activity, DeliveryResult<TKey, TValue> deliveryResult, IKafkaProducerOptions<TKey, TValue> options);

        public abstract void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerWorkerOptions<TKey, TValue> options, Exception exception = null);
    }
}
