﻿using System;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal sealed class BackgroundWorkItem<TKey, TValue>
    {
        private Exception _exception;

        public bool IsHandled { get; private set; }
        public TaskActivity TaskActivity { get; private set; }
        public ConsumeResult<TKey, TValue> ConsumeResult { get; private set; }

        public int Id => TaskActivity.ExecutingTask!.Id;
        public bool IsCompleted => TaskActivity.ExecutingTask!.IsCompleted;
        public bool IsCanceled => TaskActivity.ExecutingTask!.IsCanceled;
        public bool IsFaulted => TaskActivity.ExecutingTask!.IsFaulted;
        public AggregateException Exception => TaskActivity.ExecutingTask!.Exception;

        public BackgroundWorkItem(TaskActivity taskActivity, ConsumeResult<TKey, TValue> consumeResult)
        {
            TaskActivity = taskActivity ?? throw new ArgumentNullException(nameof(taskActivity), $"{nameof(taskActivity)} cannot be null.");
            ConsumeResult = consumeResult ?? throw new ArgumentNullException(nameof(consumeResult), $"{nameof(consumeResult)} cannot be null.");
        }

        public async Task<Exception> GetExceptionAsync()
        {
            if (!TaskActivity.ExecutingTask!.IsFaulted)
            {
                return null;
            }

            if (_exception is null)
            {
                try
                {
                    await TaskActivity.ExecutingTask!.ConfigureAwait(continueOnCapturedContext: false);
                }
                catch (Exception exception)
                {
                    _exception = exception;
                }
            }

            return _exception ?? TaskActivity.ExecutingTask!.Exception;
        }

        public void SetHandled()
        {
            IsHandled = true;
        }

        public BackgroundWorkItem<TKey, TValue> AttachContinuation<TResult>(Func<TaskActivity, TResult> continuationFunction)
        {
            TaskActivity.ContinueWith(continuationFunction, TaskContinuationOptions.AttachedToParent);

            return this;
        }
    }
}
