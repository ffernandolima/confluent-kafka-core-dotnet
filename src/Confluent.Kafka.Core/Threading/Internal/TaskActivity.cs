using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Threading.Internal
{
    internal sealed class TaskActivity
    {
        public Task ExecutingTask { get; private set; }
        public Activity TraceActivity { get; private set; }

        public TaskActivity(Task executingTask, Activity traceActivity)
        {
            ExecutingTask = executingTask ?? throw new ArgumentNullException(nameof(executingTask));
            TraceActivity = traceActivity;
        }

        public static TaskActivity Run(Func<Action<Activity>, Task> function)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            Activity executingActivity = null;

            void ActivitySetter(Activity createdActivity) => executingActivity = createdActivity;

            var executingTask = Task.Run(() => function.Invoke(ActivitySetter));

            var taskActivity = new TaskActivity(executingTask, executingActivity);

            return taskActivity;
        }

        public Task<TResult> ContinueWith<TResult>(Func<TaskActivity, TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction is null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var continuationTask = ExecutingTask.ContinueWith(_ => continuationFunction.Invoke(this), continuationOptions);

            return continuationTask;
        }
    }
}
