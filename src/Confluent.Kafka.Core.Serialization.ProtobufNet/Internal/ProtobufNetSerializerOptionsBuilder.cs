using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal sealed class ProtobufNetSerializerOptionsBuilder :
        FunctionalBuilder<ProtobufNetSerializerOptions, ProtobufNetSerializerOptionsBuilder>,
        IProtobufNetSerializerOptionsBuilder
    {
        public IProtobufNetSerializerOptionsBuilder WithAutomaticRuntimeMap(bool automaticRuntimeMap)
        {
            AppendAction(options => options.AutomaticRuntimeMap = automaticRuntimeMap);
            return this;
        }

        public static ProtobufNetSerializerOptions Build(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IProtobufNetSerializerOptionsBuilder> configureOptions)
        {
            using var builder = new ProtobufNetSerializerOptionsBuilder();

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}
