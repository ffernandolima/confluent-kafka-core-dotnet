using Confluent.Kafka.Core.Models;

namespace Confluent.Kafka.Core.Hosting.Retry
{
    public interface IKafkaRetryConsumerWorker : IKafkaConsumerWorker<byte[], KafkaMetadataMessage>
    { }
}
