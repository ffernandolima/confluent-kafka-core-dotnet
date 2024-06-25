namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerHandlerFactoryOptionsBuilder
    {
        IKafkaProducerHandlerFactoryOptionsBuilder FromConfiguration(string sectionKey);
        IKafkaProducerHandlerFactoryOptionsBuilder WithEnableLogging(bool enableLogging);
    }
}


