using System;

namespace Confluent.Kafka.Core.Models
{
    public interface IMessageValue
    {
        Guid Id { get; }
    }
}
