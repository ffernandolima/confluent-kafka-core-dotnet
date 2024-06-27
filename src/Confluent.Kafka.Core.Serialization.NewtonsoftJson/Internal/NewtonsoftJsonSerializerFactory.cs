using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal
{
    internal sealed class NewtonsoftJsonSerializerFactory
    {
        private static readonly Lazy<NewtonsoftJsonSerializerFactory> Factory = new(
           () => new NewtonsoftJsonSerializerFactory(), isThreadSafe: true);

        public static NewtonsoftJsonSerializerFactory Instance => Factory.Value;

        private NewtonsoftJsonSerializerFactory()
        { }

        public NewtonsoftJsonSerializer<T> GetOrCreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IJsonSerializerSettingsBuilder> configureSettings,
            object serializerKey)
        {
            var serializer = serviceProvider?.GetKeyedService<NewtonsoftJsonSerializer<T>>(
                serializerKey ?? NewtonsoftJsonSerializerConstants.NewtonsoftJsonSerializerKey) ??
                CreateSerializer<T>(serviceProvider, configuration, (_, builder) => configureSettings?.Invoke(builder));

            return serializer;
        }

        public NewtonsoftJsonSerializer<T> CreateSerializer<T>(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IJsonSerializerSettingsBuilder> configureSettings)
        {
            var settings = JsonSerializerSettingsBuilder.Build(
                serviceProvider,
                configuration ?? serviceProvider?.GetService<IConfiguration>(),
                configureSettings);

            var serializer = new NewtonsoftJsonSerializer<T>(settings);

            return serializer;
        }
    }
}
