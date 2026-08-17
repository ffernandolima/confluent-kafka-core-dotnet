namespace Confluent.Kafka.Core.Models.Internal
{
    internal static class KafkaSenderConstants
    {
        public const string Sender = "Sender";

        /// <summary>
        /// Value for messaging.system, shared by the span attributes and the metric attributes.
        /// </summary>
        public const string KafkaSystem = "kafka";
    }
}
