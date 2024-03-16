using Confluent.Kafka.Core.Internal;
using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Retry.Polly.Internal
{
    internal sealed class PollyRetryHandlerOptionsBuilder :
        FunctionalBuilder<PollyRetryHandlerOptions, PollyRetryHandlerOptionsBuilder>,
        IPollyRetryHandlerOptionsBuilder
    {
        public IPollyRetryHandlerOptionsBuilder WithRetryCount(int retryCount)
        {
            AppendAction(options => options.RetryCount = retryCount);
            return this;
        }

        public IPollyRetryHandlerOptionsBuilder WithRetryDelay(TimeSpan retryDelay)
        {
            AppendAction(options => options.RetryDelay = retryDelay);
            return this;
        }

        public IPollyRetryHandlerOptionsBuilder WithDelayProvider(Func<int, TimeSpan> delayProvider)
        {
            AppendAction(options => options.DelayProvider = delayProvider);
            return this;
        }

        public IPollyRetryHandlerOptionsBuilder WithDelays(IEnumerable<TimeSpan> delays)
        {
            AppendAction(options => options.Delays = delays);
            return this;
        }

        public IPollyRetryHandlerOptionsBuilder WithExceptionTypeFilters(string[] exceptionTypeFilters)
        {
            AppendAction(options => options.ExceptionTypeFilters = exceptionTypeFilters);
            return this;
        }

        public IPollyRetryHandlerOptionsBuilder WithExceptionFilter(Func<Exception, bool> exceptionFilter)
        {
            AppendAction(options => options.ExceptionFilter = exceptionFilter);
            return this;
        }

        public IPollyRetryHandlerOptionsBuilder WithEnableLogging(bool enableLogging)
        {
            AppendAction(options => options.EnableLogging = enableLogging);
            return this;
        }

        public static PollyRetryHandlerOptions Build(Action<IPollyRetryHandlerOptionsBuilder> configureOptions)
        {
            using var builder = new PollyRetryHandlerOptionsBuilder();

            configureOptions?.Invoke(builder);

            var options = builder.Build();

            return options;
        }
    }
}
