using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal sealed class TaskActivity
    {
        public Task ExecutingTask { get; private set; }
        public Activity TraceActivity { get; private set; }

        public TaskActivity(Task executingTask, Activity traceActivity)
        {
            ExecutingTask = executingTask ?? throw new ArgumentNullException(nameof(executingTask), $"{nameof(executingTask)} cannot be null.");
            TraceActivity = traceActivity;
        }

        public static TaskActivity Run(Func<Activity, Task> processWorkItem)
        {
            if (processWorkItem is null)
            {
                throw new ArgumentNullException(nameof(processWorkItem), $"{nameof(processWorkItem)} cannot be null.");
            }

            Activity traceActivity = null;

            var executingTask = Task.Run(() => processWorkItem.Invoke(traceActivity));

            var taskActivity = new TaskActivity(executingTask, traceActivity);

            return taskActivity;
        }

        public Task<TResult> ContinueWith<TResult>(Func<TaskActivity, TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction is null)
            {
                throw new ArgumentNullException(nameof(continuationFunction), $"{nameof(continuationFunction)} cannot be null.");
            }

            var continuationTask = ExecutingTask.ContinueWith(_ => continuationFunction.Invoke(this), continuationOptions);

            return continuationTask;
        }
    }
}
