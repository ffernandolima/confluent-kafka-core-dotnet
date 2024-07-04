using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Internal
{
    /// <remarks>https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md#cancelling-uncancellable-operations</remarks>
    internal static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> sourceTask, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            using (cancellationToken.Register(state => ((TaskCompletionSource<object>)state).TrySetResult(result: null), taskCompletionSource))
            {
                var completedTask = await Task.WhenAny(sourceTask, taskCompletionSource.Task);

                if (completedTask == taskCompletionSource.Task)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                return await sourceTask;
            }
        }
    }
}
