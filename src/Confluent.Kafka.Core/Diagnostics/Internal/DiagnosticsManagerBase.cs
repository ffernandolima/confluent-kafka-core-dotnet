using Confluent.Kafka.Core.Consumer;
using Confluent.Kafka.Core.Producer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal abstract class DiagnosticsManagerBase : IDiagnosticsManager
    {
        protected abstract ActivitySourceBase ActivitySource { get; }
        protected abstract ActivityEnricherBase ActivityEnricher { get; }
        protected virtual DistributedContextPropagator Propagator { get; } = DistributedContextPropagator.CreateDefaultPropagator();

        public Activity StartActivity(string activityName, ActivityKind activityKind, IPropagationContext propagationContext)
        {
            var activity = ActivitySource?.StartActivity(activityName, activityKind, propagationContext);

            return activity;
        }

        public Activity StartProducerActivity(string activityName, IDictionary<string, string> carrier)
        {
            var propagationContext = ExtractContext(carrier);

            if (propagationContext is null || propagationContext.ActivityContext == default)
            {
                var currentActivity = Activity.Current;

                propagationContext = new PropagationContext(
                    currentActivity.GetContextOrDefault(),
                    currentActivity.GetBaggageOrEmpty());
            }

            var activity = StartActivity(activityName, ActivityKind.Producer, propagationContext);

            InjectContext(activity, carrier);

            return activity;
        }

        public Activity StartConsumerActivity(string activityName, IDictionary<string, string> carrier)
        {
            var propagationContext = ExtractContext(carrier);

            var activity = StartActivity(activityName, ActivityKind.Consumer, propagationContext);

            InjectContext(activity, carrier);

            return activity;
        }

        public void InjectContext(Activity activity, IDictionary<string, string> carrier)
        {
            Propagator?.Inject(activity, carrier);
        }

        public IPropagationContext ExtractContext(IDictionary<string, string> carrier)
        {
            var propagationContext = Propagator?.Extract(carrier);

            return propagationContext;
        }

        public void Enrich(
            Activity activity,
            ConsumeException consumeException,
            IConsumerConfig consumerConfig,
            Func<byte[], object> messageIdHandler = null)
        {
            ActivityEnricher?.Enrich(activity, consumeException, consumerConfig, messageIdHandler);
        }

        public void Enrich<TKey, TValue>(
            Activity activity,
            ConsumeResult<TKey, TValue> consumeResult,
            IKafkaConsumerOptions<TKey, TValue> options)
        {
            ActivityEnricher?.Enrich(activity, consumeResult, options);
        }

        public void Enrich<TKey, TValue>(
            Activity activity,
            ProduceException<TKey, TValue> produceException,
            IKafkaProducerOptions<TKey, TValue> options)
        {
            ActivityEnricher?.Enrich(activity, produceException, options);
        }

        public void Enrich<TKey, TValue>(
            Activity activity,
            DeliveryReport<TKey, TValue> deliveryReport,
            IKafkaProducerOptions<TKey, TValue> options)
        {
            ActivityEnricher?.Enrich(activity, deliveryReport, options);
        }

        public void Enrich<TKey, TValue>(
            Activity activity,
            DeliveryResult<TKey, TValue> deliveryResult,
            IKafkaProducerOptions<TKey, TValue> options)
        {
            ActivityEnricher?.Enrich(activity, deliveryResult, options);
        }
    }
}
