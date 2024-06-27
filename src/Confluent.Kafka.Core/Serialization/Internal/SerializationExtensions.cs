using Confluent.Kafka.SyncOverAsync;

namespace Confluent.Kafka.Core.Serialization.Internal
{
    internal static class SerializationExtensions
    {
        public static ISerializer<T> ToSerializer<T>(this object sourceObject)
        {
            if (sourceObject is ISerializer<T> serializer)
            {
                return serializer;
            }

            if (sourceObject is SyncOverAsyncDeserializer<T> syncDeserializer)
            {
                var innerDeserializer = syncDeserializer.GetInnerDeserializer();

                if (innerDeserializer is IAsyncSerializer<T> asyncSerializer)
                {
                    return asyncSerializer.AsSyncOverAsync();
                }
            }

            return KafkaSerialization.TryGetSerializer<T>();
        }
    }
}
