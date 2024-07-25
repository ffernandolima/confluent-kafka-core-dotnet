using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IKafkaConsumerLifecycleWorker<TKey, TValue>
    {
        Task StartAsync(IKafkaConsumerWorkerOptions<TKey, TValue> options, CancellationToken cancellationToken);

        Task StopAsync(IKafkaConsumerWorkerOptions<TKey, TValue> options, CancellationToken cancellationToken);
    }
}
