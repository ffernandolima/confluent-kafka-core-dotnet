using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerHandlerFactoryOptionsBuilder :
        FunctionalBuilder<KafkaConsumerHandlerFactoryOptions, KafkaConsumerHandlerFactoryOptionsBuilder>,
        IKafkaConsumerHandlerFactoryOptionsBuilder
    {
        public IKafkaConsumerHandlerFactoryOptionsBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(options => options.EnableLogging = enableLogging);
            return this;
        }

        public static KafkaConsumerHandlerFactoryOptions Build(
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions)
        {
            using var builder = new KafkaConsumerHandlerFactoryOptionsBuilder();

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}
