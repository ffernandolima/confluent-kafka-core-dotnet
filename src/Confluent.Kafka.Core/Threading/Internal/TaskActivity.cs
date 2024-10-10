using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class TaskActivity
    {
        public Task ExecutingTask { get; private set; }
        public Activity TraceActivity { get; private set; }

        public TaskActivity(Task executingTask, Activity traceActivity = null)
        {
            ExecutingTask = executingTask ?? throw new ArgumentNullException(nameof(executingTask));
            TraceActivity = traceActivity;
        }

        public static TaskActivity Run(Func<Task<Activity>> function)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            var executingTask = Task.Run(function.Invoke);

            var taskActivity = new TaskActivity(executingTask);

            executingTask.ContinueWith(completedTask =>
            {
                taskActivity.TraceActivity = completedTask?.Result;
                taskActivity.TraceActivity?.SetEndTime(DateTime.UtcNow);

            }, TaskContinuationOptions.AttachedToParent);

            return taskActivity;
        }
    }
}
