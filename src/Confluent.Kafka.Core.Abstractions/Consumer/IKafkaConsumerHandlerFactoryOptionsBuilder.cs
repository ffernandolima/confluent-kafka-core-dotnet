namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerHandlerFactoryOptionsBuilder
    {
        IKafkaConsumerHandlerFactoryOptionsBuilder FromConfiguration(string sectionKey);
        IKafkaConsumerHandlerFactoryOptionsBuilder WithEnableLogging(bool enableLogging);
    }
}
