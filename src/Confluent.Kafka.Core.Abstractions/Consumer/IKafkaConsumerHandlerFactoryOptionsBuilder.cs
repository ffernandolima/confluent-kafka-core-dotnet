namespace Confluent.Kafka.Core.Consumer
{
    public interface IKafkaConsumerHandlerFactoryOptionsBuilder
    {
        IKafkaConsumerHandlerFactoryOptionsBuilder WithEnableLogging(bool enableLogging);
    }
}
