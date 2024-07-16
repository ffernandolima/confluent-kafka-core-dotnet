using System;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal static class MessageExtensions
    {
        public static void EnsureDefaultMetadata<TKey, TValue>(this Message<TKey, TValue> message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Headers ??= [];

            if (message.Timestamp == Timestamp.Default)
            {
                message.Timestamp = new Timestamp(dateTime: DateTime.UtcNow);
            }
        }

        public static object GetId<TKey, TValue>(this Message<TKey, TValue> message, Func<TValue, object> handler)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var messageId = handler?.Invoke(message.Value);

            return messageId;
        }
    }
}
