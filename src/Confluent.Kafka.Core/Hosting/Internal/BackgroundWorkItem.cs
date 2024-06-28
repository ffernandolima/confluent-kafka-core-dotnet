using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal sealed class BackgroundWorkItem<TKey, TValue>
    {
        private Exception _exception;

        public bool IsHandled { get; private set; }
        public Task BackgroundTask { get; private set; }
        public Activity Activity { get; private set; }
        public ConsumeResult<TKey, TValue> ConsumeResult { get; private set; }

        public int Id => BackgroundTask.Id;
        public bool IsCompleted => BackgroundTask.IsCompleted;
        public bool IsCanceled => BackgroundTask.IsCanceled;
        public bool IsFaulted => BackgroundTask.IsFaulted;
        public AggregateException Exception => BackgroundTask.Exception;

        public BackgroundWorkItem(Task backgroundTask, Activity activity, ConsumeResult<TKey, TValue> consumeResult)
        {
            BackgroundTask = backgroundTask ?? throw new ArgumentNullException(nameof(backgroundTask), $"{nameof(backgroundTask)} cannot be null.");
            Activity = activity ?? throw new ArgumentNullException(nameof(activity), $"{nameof(activity)} cannot be null.");
            ConsumeResult = consumeResult ?? throw new ArgumentNullException(nameof(consumeResult), $"{nameof(consumeResult)} cannot be null.");
        }

        public async Task<Exception> GetExceptionAsync()
        {
            if (!BackgroundTask.IsFaulted)
            {
                return null;
            }

            if (_exception is null)
            {
                try
                {
                    await BackgroundTask.ConfigureAwait(continueOnCapturedContext: false);
                }
                catch (Exception exception)
                {
                    _exception = exception;
                }
            }

            return _exception ?? BackgroundTask.Exception;
        }

        public void SetHandled()
        {
            IsHandled = true;
        }

        public BackgroundWorkItem<TKey, TValue> AttachContinuation<TResult>(Func<Task, TResult> continuation)
        {
            if (continuation is null)
            {
                throw new ArgumentNullException(nameof(continuation), $"{nameof(continuation)} cannot be null.");
            }

            BackgroundTask.ContinueWith(continuation, TaskContinuationOptions.AttachedToParent);

            return this;
        }
    }
}
