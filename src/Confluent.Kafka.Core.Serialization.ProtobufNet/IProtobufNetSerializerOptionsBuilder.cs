namespace Confluent.Kafka.Core.Serialization.ProtobufNet
{
    public interface IProtobufNetSerializerOptionsBuilder
    {
        IProtobufNetSerializerOptionsBuilder WithAutomaticRuntimeMap(bool automaticRuntimeMap);
    }
}
