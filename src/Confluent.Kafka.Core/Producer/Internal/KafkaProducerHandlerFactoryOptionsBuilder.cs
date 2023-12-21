using System;
using Confluent.Kafka.Core.Internal;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerHandlerFactoryOptionsBuilder :
        FunctionalBuilder<KafkaProducerHandlerFactoryOptions, KafkaProducerHandlerFactoryOptionsBuilder>,
        IKafkaProducerHandlerFactoryOptionsBuilder
    {
        public IKafkaProducerHandlerFactoryOptionsBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(options => options.EnableLogging = enableLogging);
            return this;
        }

        internal static KafkaProducerHandlerFactoryOptions Build(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions)
        {
            using var builder = new KafkaProducerHandlerFactoryOptionsBuilder();

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}

