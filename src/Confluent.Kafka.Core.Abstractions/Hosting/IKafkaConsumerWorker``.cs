using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IKafkaConsumerWorker<TKey, TValue> : IDisposable
    {
        IKafkaConsumerWorkerOptions<TKey, TValue> Options { get; }

        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
