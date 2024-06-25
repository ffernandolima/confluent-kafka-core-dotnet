using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal sealed class KafkaProducerHandlerFactoryOptionsBuilder :
        FunctionalBuilder<KafkaProducerHandlerFactoryOptions, KafkaProducerHandlerFactoryOptionsBuilder>,
        IKafkaProducerHandlerFactoryOptionsBuilder
    {
        public KafkaProducerHandlerFactoryOptionsBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        { }

        public IKafkaProducerHandlerFactoryOptionsBuilder FromConfiguration(string sectionKey)
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

        public IKafkaProducerHandlerFactoryOptionsBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(options => options.EnableLogging = enableLogging);
            return this;
        }

        public static KafkaProducerHandlerFactoryOptions Build(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Action<IServiceProvider, IKafkaProducerHandlerFactoryOptionsBuilder> configureOptions)
        {
            using var builder = new KafkaProducerHandlerFactoryOptionsBuilder(configuration);

            configureOptions?.Invoke(serviceProvider, builder);

            var options = builder.Build();

            return options;
        }
    }
}

