using System;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal static class MessageValueExtensions
    {
        public static Guid? GetId(this IMessageValue messageValue)
        {
            var messageId = messageValue is not null && !messageValue.Id.Equals(Guid.Empty)
                ? new Guid?(messageValue.Id)
                : null;

            return messageId;
        }
    }
}
