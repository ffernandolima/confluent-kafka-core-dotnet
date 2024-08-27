using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class ProtobufNetSerializerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddProtobufNetSerializer<T>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services!.AddProtobufNetSerializer<T>(configureOptions, serializerKey);

            return builder;
        }
    }
}
