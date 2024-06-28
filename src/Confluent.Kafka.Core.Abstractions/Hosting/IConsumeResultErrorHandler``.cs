using System;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IConsumeResultErrorHandler<TKey, TValue>
    {
        Task HandleAsync(ConsumeResult<TKey, TValue> consumeResult, Exception exception);
    }
}
