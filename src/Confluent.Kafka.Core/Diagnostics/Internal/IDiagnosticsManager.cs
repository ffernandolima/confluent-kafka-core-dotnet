using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Producer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal interface IDiagnosticsManager
    {
        Activity StartProducerActivity(string activityName, IDictionary<string, string> carrier);

        Activity StartConsumerActivity(string activityName, IDictionary<string, string> carrier);

        Activity StartActivity(string activityName, ActivityKind activityKind, PropagationContext context);

        void InjectContext(Activity activity, IDictionary<string, string> carrier);

        PropagationContext ExtractContext(IDictionary<string, string> carrier);

        void Enrich(Activity activity, ConsumeException consumeException, IConsumerConfig consumerConfig, Func<byte[], object> messageIdHandler = null);

        void Enrich<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> consumeResult, IKafkaConsumerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, ProduceException<TKey, TValue> produceException, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, DeliveryReport<TKey, TValue> deliveryReport, IKafkaProducerOptions<TKey, TValue> options);

        void Enrich<TKey, TValue>(Activity activity, DeliveryResult<TKey, TValue> deliveryResult, IKafkaProducerOptions<TKey, TValue> options);
    }
}
