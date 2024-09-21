using Confluent.Kafka.Core.Consumer;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Tests.Core.Extensions
{
    public static class KafkaConsumerExtensions
    {
        public static ConsumeResult<TKey, TValue> Consume<TKey, TValue>(
            this IKafkaConsumer<TKey, TValue> consumer,
            TimeSpan timeout,
            int retryCount)
        {
            var consumeResult = Policy<ConsumeResult<TKey, TValue>>
                .HandleResult(result => result is null)
                .WaitAndRetry(retryCount, retryAttempt => timeout)
                .Execute(consumer.Consume);

            return consumeResult;
        }

        public static IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch<TKey, TValue>(
            this IKafkaConsumer<TKey, TValue> consumer,
            TimeSpan timeout,
            int retryCount)
        {
            var consumeResults = Policy<IEnumerable<ConsumeResult<TKey, TValue>>>
                .HandleResult(result => result is null || !result.Any())
                .WaitAndRetry(retryCount, retryAttempt => timeout)
                .Execute(consumer.ConsumeBatch);

            return consumeResults;
        }
    }
}
