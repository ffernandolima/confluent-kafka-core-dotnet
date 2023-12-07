using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerService : BackgroundService, IKafkaConsumerService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
