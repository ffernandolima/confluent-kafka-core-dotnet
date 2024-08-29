using Confluent.Kafka.Core.DependencyInjection;
using Confluent.Kafka.Core.DependencyInjection.Internal;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaServiceCollectionExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, Action<IKafkaBuilder> configureKafka)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureKafka is null)
            {
                throw new ArgumentNullException(nameof(configureKafka));
            }

            var builder = new KafkaBuilder(services);

            configureKafka.Invoke(builder);

            builder.AddKafkaDiagnostics();

            return services;
        }
    }
}
