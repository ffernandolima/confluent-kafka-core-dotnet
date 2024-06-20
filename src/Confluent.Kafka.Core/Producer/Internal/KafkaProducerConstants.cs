namespace Confluent.Kafka.Core.Producer.Internal
{
    internal static class KafkaProducerConstants
    {
        public const string RetryProducer = "RetryProducer";
        public const string DeadLetterProducer = "DeadLetterProducer";
        public const string DeadLetterTopicSuffix = ".DeadLetter";
    }
}
