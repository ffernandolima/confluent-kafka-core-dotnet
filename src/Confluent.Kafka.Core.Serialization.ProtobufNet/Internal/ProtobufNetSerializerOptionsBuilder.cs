using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal sealed class ProtobufNetSerializerOptionsBuilder :
        FunctionalBuilder<ProtobufNetSerializerOptions, ProtobufNetSerializerOptionsBuilder>,
        IProtobufNetSerializerOptionsBuilder
    {
        public ProtobufNetSerializerOptionsBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public IProtobufNetSerializerOptionsBuilder FromConfiguration(string sectionKey)
        {
            AppendAction(options =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    options = Bind(options, sectionKey);
                }
            });
            return this;
        }

        public IProtobufNetSerializerOptionsBuilder WithAutomaticRuntimeMap(bool automaticRuntimeMap)
        {
            AppendAction(options => options.AutomaticRuntimeMap = automaticRuntimeMap);
            return this;
        }

        public static ProtobufNetSerializerOptions Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions)
        {
            using var builder = new ProtobufNetSerializerOptionsBuilder(configuration);

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}
