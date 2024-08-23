using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public interface IConsumeResultHandler<TKey, TValue>
    {
        Task HandleAsync(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken);
    }
}
