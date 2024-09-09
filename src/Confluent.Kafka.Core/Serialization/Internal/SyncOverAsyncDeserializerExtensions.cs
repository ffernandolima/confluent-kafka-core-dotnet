using Confluent.Kafka.SyncOverAsync;
using System.Reflection;

namespace Confluent.Kafka.Core.Serialization.Internal
{
    internal static class SyncOverAsyncDeserializerExtensions
    {
        public static IAsyncDeserializer<T> GetInnerDeserializer<T>(this SyncOverAsyncDeserializer<T> syncOverAsyncDeserializer)
        {
            IAsyncDeserializer<T> innerDeserializer = null;

            if (syncOverAsyncDeserializer is not null)
            {
                innerDeserializer = (IAsyncDeserializer<T>)typeof(SyncOverAsyncDeserializer<T>)
                     .GetProperty("asyncDeserializer", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(syncOverAsyncDeserializer);
            }

            return innerDeserializer;
        }
    }
}
