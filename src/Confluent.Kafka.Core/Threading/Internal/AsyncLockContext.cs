using System.Collections.Generic;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class AsyncLockContext
    {
        public static readonly AsyncLockContext Empty = new();

        public IDictionary<object, object> Items { get; } = new Dictionary<object, object>();

        public object this[object key]
        {
            get => Items[key];
            set => Items[key] = value;
        }
    }
}
