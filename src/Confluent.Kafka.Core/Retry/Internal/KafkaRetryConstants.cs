namespace Confluent.Kafka.Core.Retry.Internal
{
    internal static class KafkaRetryConstants
    {
        public const string RetryHandler = "RetryHandler";
        public const string RetryCountKey = "X-Retry-Count";
        public const string RetryGroupIdKey = "X-Retry-Group-Id";
        public const string RetryTopicSuffix = ".Retry";
    }
}
