using Microsoft.Extensions.Hosting;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IKafkaConsumerWorker<TKey, TValue> : IHostedService
    {
        IKafkaConsumerWorkerOptions<TKey, TValue> Options { get; }
    }
}
