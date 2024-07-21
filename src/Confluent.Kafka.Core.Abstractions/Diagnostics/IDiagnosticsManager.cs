using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Hosting;
using Confluent.Kafka.Core.Producer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics
{
    public interface IDiagnosticsManager
    {
        Activity StartActivity(string activityName, ActivityKind activityKind, IPropagationContext propagationContext);

        Activity StartProducerActivity(string activityName, IDictionary<string, string> carrier);

        Activity StartConsumerActivity(string activityName, IDictionary<string, string> carrier);

        void InjectContext(Activity activity, IDictionary<string, string> carrier);

        IPropagationContext ExtractContext(IDictionary<string, string> carrier);

        void Enrich(Activity activity, ConsumeException consumeException, IKafkaConsumerConfig consumerConfig);

        void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, ProduceException<TKey, TValue> produceException, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, DeliveryReport<TKey, TValue> deliveryReport, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, DeliveryResult<TKey, TValue> deliveryResult, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, Exception exception, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerWorkerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerWorkerOptions<TKey, TValue> options);
    }
}
