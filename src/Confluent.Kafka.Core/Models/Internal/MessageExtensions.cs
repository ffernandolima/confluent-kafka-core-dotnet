using System;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal static class MessageExtensions
    {
        public static void EnsureDefaultMetadata<TKey, TValue>(this Message<TKey, TValue> message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message), $"{nameof(message)} cannot be null.");
            }

            message.Headers ??= new Headers();

            if (message.Timestamp == Timestamp.Default)
            {
                message.Timestamp = new Timestamp(dateTime: DateTime.UtcNow);
            }
        }
    }
}
