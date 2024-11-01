﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Retry
{
    public interface IRetryHandler<TKey, TValue>
    {
        void Handle(
            Action<CancellationToken> executeAction,
            Action<Exception, TimeSpan, int> onRetryAction = null,
            CancellationToken cancellationToken = default);

        Task HandleAsync(
            Func<CancellationToken, Task> executeAction,
            Action<Exception, TimeSpan, int> onRetryAction = null,
            CancellationToken cancellationToken = default);
    }
}
