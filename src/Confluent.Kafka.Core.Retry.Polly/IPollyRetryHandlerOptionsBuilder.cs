using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Retry.Polly
{
    public interface IPollyRetryHandlerOptionsBuilder
    {
        IPollyRetryHandlerOptionsBuilder FromConfiguration(string sectionKey);

        IPollyRetryHandlerOptionsBuilder WithRetryCount(int retryCount);

        IPollyRetryHandlerOptionsBuilder WithRetryDelay(TimeSpan retryDelay);

        IPollyRetryHandlerOptionsBuilder WithDelayProvider(Func<int, TimeSpan> delayProvider);

        IPollyRetryHandlerOptionsBuilder WithDelays(IEnumerable<TimeSpan> delays);

        IPollyRetryHandlerOptionsBuilder WithExceptionTypeFilters(string[] exceptionTypeFilters);

        IPollyRetryHandlerOptionsBuilder WithExceptionFilter(Func<Exception, bool> exceptionFilter);

        IPollyRetryHandlerOptionsBuilder WithEnableLogging(bool enableLogging);
    }
}
