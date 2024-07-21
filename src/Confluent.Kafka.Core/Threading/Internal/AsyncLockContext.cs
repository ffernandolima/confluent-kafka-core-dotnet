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
    }
}
