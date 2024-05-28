using Confluent.Kafka.Core.Consumer;
using Microsoft.Extensions.DependencyInjection;

namespace Confluent.Kafka.Core.Tests
{
    class Program
    {
        static void Main(string[] _)
        {
            // IKafkaConsumerBuilder<string, object> builder = new KafkaConsumerBuilder<string, object>();

            var services = new ServiceCollection()
                .AddKafkaConsumer<string, object>((_, builder) =>
                    builder.WithConsumerConfiguration((_, builder) => builder.WithBootstrapServers("localhost:9092"))
                           .WithJsonCoreValueDeserializer());

            using var serviceProvider = services.BuildServiceProvider();

            var consumer = serviceProvider.GetRequiredService<IKafkaConsumer<string, object>>();
        }
    }
}
