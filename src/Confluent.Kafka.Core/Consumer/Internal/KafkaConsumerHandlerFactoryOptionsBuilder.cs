using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumerHandlerFactoryOptionsBuilder :
        FunctionalBuilder<KafkaConsumerHandlerFactoryOptions, KafkaConsumerHandlerFactoryOptionsBuilder>,
        IKafkaConsumerHandlerFactoryOptionsBuilder
    {
        public KafkaConsumerHandlerFactoryOptionsBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public IKafkaConsumerHandlerFactoryOptionsBuilder FromConfiguration(string sectionKey)
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

        public IKafkaConsumerHandlerFactoryOptionsBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(options => options.EnableLogging = enableLogging);
            return this;
        }

        public static KafkaConsumerHandlerFactoryOptions Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IKafkaConsumerHandlerFactoryOptionsBuilder> configureOptions)
        {
            using var builder = new KafkaConsumerHandlerFactoryOptionsBuilder(configuration);

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}
