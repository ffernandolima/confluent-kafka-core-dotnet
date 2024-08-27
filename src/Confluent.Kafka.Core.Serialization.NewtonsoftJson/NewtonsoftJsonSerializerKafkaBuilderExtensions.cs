using Confluent.Kafka.Core.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.DependencyInjection
{
    public static class NewtonsoftJsonSerializerKafkaBuilderExtensions
    {
        public static IKafkaBuilder AddNewtonsoftJsonSerializer<T>(
            this IKafkaBuilder builder,
            Action<IServiceProvider, IJsonSerializerSettingsBuilder> configureSettings = null,
            object serializerKey = null)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services!.AddNewtonsoftJsonSerializer<T>(configureSettings, serializerKey);

            return builder;
        }
    }
}
