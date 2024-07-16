using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class AsyncLockContext
    {
        public IDictionary<object, object> Items { get; }

        public object this[object key]
        {
            get => Items[key];
            set => Items[key] = value;
        }

        public AsyncLockContext(params KeyValuePair<object, object>[] items)
        {
            Items = new ConcurrentDictionary<object, object>(items);
        }

        public static AsyncLockContext Create(object key, object value)
            => new(new KeyValuePair<object, object>(key, value));
    }
}
