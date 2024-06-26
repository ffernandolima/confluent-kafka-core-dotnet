namespace Confluent.Kafka.Core.Serialization.ProtobufNet
{
    public interface IProtobufNetSerializerOptionsBuilder
    {
        IProtobufNetSerializerOptionsBuilder FromConfiguration(string sectionKey);

        IProtobufNetSerializerOptionsBuilder WithAutomaticRuntimeMap(bool automaticRuntimeMap);
    }
}
