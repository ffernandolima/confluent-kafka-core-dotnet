using Confluent.Kafka.Core.Serialization.JsonCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class JsonCoreSerializerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddJsonCoreSerializer<T>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, IJsonSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services!.AddJsonCoreSerializer<T>(configureOptions, serializerKey);

            return builder;
        }
    }
}
