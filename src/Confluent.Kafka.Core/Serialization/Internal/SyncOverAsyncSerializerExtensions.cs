using Confluent.Kafka.SyncOverAsync;
using System.Reflection;

namespace Confluent.Kafka.Core.Serialization.Internal
{
    internal static class SyncOverAsyncSerializerExtensions
    {
        public static IAsyncSerializer<T> GetInnerSerializer<T>(this SyncOverAsyncSerializer<T> syncOverAsyncSerializer)
        {
            IAsyncSerializer<T> innerSerializer = null;

            if (syncOverAsyncSerializer is not null)
            {
                innerSerializer = (IAsyncSerializer<T>)typeof(SyncOverAsyncSerializer<T>)
                     .GetProperty("asyncSerializer", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(syncOverAsyncSerializer);
            }

            return innerSerializer;
        }
    }
}
