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

        internal static ProtobufNetSerializerOptions Build(Action<IProtobufNetSerializerOptionsBuilder> configureOptions)
        {
            using var builder = new ProtobufNetSerializerOptionsBuilder();

            configureOptions?.Invoke(builder);

            var options = builder.Build();

            return options;
        }
    }
}
