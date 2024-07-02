namespace Confluent.Kafka.Core.Models.Internal
{
    internal sealed class KafkaSender
    {
        public object Instance { get; }
        public KafkaSenderType Type { get; }

        public KafkaSender(object instance, KafkaSenderType type)
        {
            Instance = instance;
            Type = type;
        }
    }
}
