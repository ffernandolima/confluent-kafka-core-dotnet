using System;

namespace Confluent.Kafka.Core.Models.Internal
{
    internal static class MessageValueExtensions
    {
        public static Guid? GetId(this IMessageValue messageValue)
        {
            return messageValue is not null && !messageValue.Id.Equals(Guid.Empty)
                 ? messageValue.Id
                 : null;
        }
    }
}
