using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Conversion.Internal;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry.Internal;
using Confluent.Kafka.Core.Threading.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal class KafkaConsumerWorker<TKey, TValue> : IKafkaConsumerWorker<TKey, TValue>
    {
        private readonly IKafkaConsumerWorkerOptions<TKey, TValue> _options;

        protected ILogger Logger { get; }
        protected string ServiceName { get; }
        protected AsyncLock AsyncLock { get; }
        protected ConcurrentBag<Exception> Exceptions { get; }
        protected ConcurrentQueue<BackgroundWorkItem<TKey, TValue>> WorkItems { get; }

        public KafkaConsumerWorker(IKafkaConsumerWorkerBuilder<TKey, TValue> builder)
            : this(
                  builder?.ToOptions<IKafkaConsumerWorkerOptions<TKey, TValue>>())
        { }

        public KafkaConsumerWorker(IKafkaConsumerWorkerOptions<TKey, TValue> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Logger = options.LoggerFactory.CreateLogger(options.WorkerConfig!.EnableLogging, options.WorkerType);
            ServiceName = options.WorkerType!.ExtractTypeName();
            AsyncLock = AsyncLockFactory.Instance.CreateAsyncLock(options.ToLockOptions());
            Exceptions = [];
            WorkItems = [];
            _options = options;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            Logger.LogWorkerStarting(ServiceName);

            if (_options.ConsumerLifecycleWorker is not null)
            {
                await _options.ConsumerLifecycleWorker.StartAsync(_options, cancellationToken).ConfigureAwait(false);
            }

            if (_options.IdempotencyHandler is not null)
            {
                await _options.IdempotencyHandler.StartAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            Logger.LogWorkerStopping(ServiceName);

            if (_options.ConsumerLifecycleWorker is not null)
            {
                await _options.ConsumerLifecycleWorker.StopAsync(_options, cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogWorkerExecuting(ServiceName);

            try
            {
                var dispatched = false;

                var hasAvailableSlots = false;

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await HandleCompletedWorkItemsAsync().ConfigureAwait(false);

                        ExecuteInternal(stoppingToken);

                        HandleConsumptionExceptions();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWorkerExecutionFailure(ex, ServiceName);
                    }

                    var delay = GetDelay(hasAvailableSlots, dispatched);

                    Logger.LogDelayingUntil(DateTime.UtcNow.Add(delay));

                    try
                    {
                        await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            finally
            {
                var delay = _options.WorkerConfig!.PendingProcessingDelay;

                while (!WorkItems.IsEmpty || !Exceptions.IsEmpty)
                {
                    try
                    {
                        await HandleCompletedWorkItemsAsync().ConfigureAwait(false);

                        HandleConsumptionExceptions();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWorkerExecutionFailure(ex, ServiceName);
                    }

                    Logger.LogDelayingUntil(DateTime.UtcNow.Add(delay));

                    await Task.Delay(delay, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        protected virtual void ExecuteInternal(CancellationToken cancellationToken)
        {
            if (AsyncLock.CurrentCount <= 0)
            {
                Logger.LogNoAvailableSlots();

                return;
            }

            IEnumerable<ConsumeResult<TKey, TValue>> consumeResults = null;

            try
            {
                consumeResults = _options.Consumer!.ConsumeBatch(AsyncLock.CurrentCount);
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);

                return;
            }

            if (consumeResults is null || !consumeResults.Any())
            {
                Logger.LogNoAvailableMessages();

                return;
            }

            foreach (var consumeResult in consumeResults.Where(consumeResult => consumeResult!.Message is not null))
            {
                DispatchWorkItem(consumeResult, cancellationToken);
            }
        }

        protected virtual async Task HandleCompletedWorkItemsAsync()
        {
            if (WorkItems.IsEmpty)
            {
                return;
            }

            try
            {
                while (WorkItems.TryPeek(out BackgroundWorkItem<TKey, TValue> workItem))
                {
                    if (!workItem.IsCompleted || !WorkItems.TryDequeue(out workItem))
                    {
                        break;
                    }

                    if (!workItem.IsFaulted && !workItem.IsCanceled)
                    {
                        Commit(workItem);

                        StoreOffset(workItem);

                        if (!workItem.IsHandled)
                        {
                            HandleCompletedWorkItem(workItem);
                        }
                    }
                    else
                    {
                        if (_options.WorkerConfig!.CommitFaultedMessages)
                        {
                            Commit(workItem);

                            StoreOffset(workItem);
                        }

                        if (!workItem.IsHandled)
                        {
                            await HandleFaultedWorkItem(workItem).ConfigureAwait(false);
                        }
                    }
                }

                var enumerator = WorkItems.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var workItem = enumerator.Current;

                    if (!workItem.IsCompleted || workItem.IsHandled)
                    {
                        continue;
                    }

                    if (!workItem.IsFaulted && !workItem.IsCanceled)
                    {
                        HandleCompletedWorkItem(workItem);
                    }
                    else
                    {
                        await HandleFaultedWorkItem(workItem).ConfigureAwait(false);
                    }
                }

                void Commit(BackgroundWorkItem<TKey, TValue> workItem)
                {
                    try
                    {
                        _options.Consumer!.Commit(workItem.ConsumeResult);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogMessageCommitFailure(ex, workItem.MessageId);
                    }
                }

                void StoreOffset(BackgroundWorkItem<TKey, TValue> workItem)
                {
                    try
                    {
                        if (!_options.Consumer!.Options!.ConsumerConfig!.EnableAutoOffsetStore.GetValueOrDefault(defaultValue: true))
                        {
                            _options.Consumer!.StoreOffset(workItem.ConsumeResult);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogMessageOffsetStorageFailure(ex, workItem.MessageId);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogCompletionHandlingFailure(ex);
            }
        }

        protected virtual void HandleConsumptionExceptions()
        {
            if (Exceptions.IsEmpty)
            {
                return;
            }

            try
            {
                while (Exceptions.TryTake(out Exception exception))
                {
                    Logger.LogMessageConsumptionFailure(exception);
                }
            }
            catch (Exception ex)
            {
                Logger.LogExceptionHandlingFailure(ex);
            }
        }

        protected virtual void DispatchWorkItem(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken)
        {
            TaskActivity taskActivity = null;

            var messageId = consumeResult.Message!.GetId(_options.Consumer!.Options!.MessageIdHandler);

            var headers = consumeResult.Message!.Headers?.ToDictionary();

            try
            {
                taskActivity = TaskActivity.Run(async activitySetter =>
                {
                    var stopwatch = Stopwatch.StartNew();

                    var activity = StartActivity(consumeResult.Topic, headers);

                    activitySetter?.Invoke(activity);

                    var lockContext = new AsyncLockContext { [ConsumeResultConstants.ConsumeResult] = consumeResult };

                    using var releaser = await AsyncLock.LockAsync(lockContext, cancellationToken).ConfigureAwait(false);

                    try
                    {
                        await BeforeProcessingAsync(consumeResult, cancellationToken).ConfigureAwait(false);

                        Logger.LogCurrentThreadBlocked(messageId, stopwatch.Elapsed);

                        if (_options.WorkerConfig!.EnableIdempotency)
                        {
                            Logger.LogIdempotencyEnabled(messageId);

                            if (ShouldBypassIdempotency(consumeResult))
                            {
                                Logger.LogIdempotencyBypassed(messageId);
                            }
                            else if (!await _options.IdempotencyHandler!.TryHandleAsync(consumeResult.Message!.Value, cancellationToken)
                                .ConfigureAwait(false))
                            {
                                Logger.LogMessageAlreadyProcessed(messageId);

                                return;
                            }
                        }
                        else
                        {
                            Logger.LogIdempotencyDisabled(messageId);

                            if (!ShouldHandleFetchedConsumeResult(consumeResult))
                            {
                                Logger.LogMessageProcessingSkip(messageId);

                                return;
                            }
                        }

                        if (_options.WorkerConfig!.EnableRetryOnFailure)
                        {
                            Logger.LogRetryStrategyEnabled(messageId);

                            await _options.RetryHandler!.HandleAsync(
                                executeAction: async cancellationToken =>
                                    await HandleFetchedConsumeResultAsync(consumeResult, cancellationToken).ConfigureAwait(false),
                                cancellationToken: cancellationToken,
                                onRetryAction: (exception, timeSpan, retryAttempt) =>
                                    Logger.LogMessageProcessingRetryFailure(exception, messageId, retryAttempt))
                            .ConfigureAwait(false);
                        }
                        else
                        {
                            Logger.LogRetryStrategyDisabled(messageId);

                            await HandleFetchedConsumeResultAsync(consumeResult, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        await AfterProcessingAsync(consumeResult, cancellationToken).ConfigureAwait(false);
                    }

                    async Task HandleFetchedConsumeResultAsync(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken)
                    {
                        foreach (var consumeResultHandler in _options.ConsumeResultHandlers)
                        {
                            await consumeResultHandler.HandleAsync(consumeResult, cancellationToken).ConfigureAwait(false);
                        }
                    }
                });
            }
            finally
            {
                if (taskActivity is not null)
                {
                    Logger.LogWorkItemDispatched(
                        messageId,
                        consumeResult.Topic,
                        consumeResult.Partition,
                        consumeResult.Offset,
                        consumeResult.Message!.Value,
                        headers);

                    var workItem = new BackgroundWorkItem<TKey, TValue>(messageId, taskActivity, consumeResult)
                        .AttachContinuation(taskActivity => taskActivity.TraceActivity?.SetEndTime(DateTime.UtcNow));

                    WorkItems.Enqueue(workItem);
                }
            }
        }

        protected virtual Task BeforeProcessingAsync(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task AfterProcessingAsync(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual void HandleCompletedWorkItem(BackgroundWorkItem<TKey, TValue> workItem)
        {
            var messageId = workItem.MessageId;

            var consumeResult = workItem.ConsumeResult;

            var activity = workItem.TaskActivity!.TraceActivity;

            try
            {
                Logger.LogMessageProcessingSuccess(messageId);

                _options.DiagnosticsManager!.Enrich(activity, consumeResult, _options);
            }
            finally
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
                activity?.Dispose();

                workItem.SetHandled();
            }
        }

        protected virtual async Task HandleFaultedWorkItem(BackgroundWorkItem<TKey, TValue> workItem)
        {
            var messageId = workItem.MessageId;

            var consumeResult = workItem.ConsumeResult;

            var activity = workItem.TaskActivity!.TraceActivity;

            var exception = await workItem.GetExceptionAsync().ConfigureAwait(false);

            try
            {
                Logger.LogMessageProcessingFailure(exception, messageId);

                _options.DiagnosticsManager!.Enrich(activity, exception, consumeResult, _options);

                if (_options.WorkerConfig!.EnableRetryTopic && ShouldProduceRetryMessage(exception, messageId))
                {
                    await ProduceRetryMessageAsync(consumeResult, messageId, exception).ConfigureAwait(false);
                }
                else if (_options.WorkerConfig!.EnableDeadLetterTopic)
                {
                    await ProduceDeadLetterMessageAsync(consumeResult, messageId, exception).ConfigureAwait(false);
                }
            }
            finally
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                activity?.Dispose();

                workItem.SetHandled();

                if (_options.ConsumeResultErrorHandler is not null)
                {
                    try
                    {
                        await _options.ConsumeResultErrorHandler.HandleAsync(consumeResult, exception).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogErrorHandlerFailure(ex, messageId);
                    }
                }
            }
        }

        protected virtual bool ShouldProduceRetryMessage(Exception exception, object messageId)
        {
            var shouldProduce = _options.WorkerConfig!.RetryTopicSpecification!.IsSatisfiedBy(exception);

            if (!shouldProduce)
            {
                Logger.LogMessageProcessingNotRetriable(messageId);
            }

            return shouldProduce;
        }

        protected virtual async Task ProduceRetryMessageAsync(ConsumeResult<TKey, TValue> consumeResult, object messageId, Exception exception = null)
        {
            var headers = consumeResult.Message!.Headers?.ToDictionary();

            headers?.AddOrUpdate(KafkaRetryConstants.RetryGroupIdKey, _options.Consumer!.Options!.ConsumerConfig!.GroupId);

            var message = CreateMetadataMessage(consumeResult, messageId, exception);

            var producerConfig = _options.RetryProducer!.Options!.ProducerConfig;

            var retryTopic = !string.IsNullOrWhiteSpace(producerConfig!.DefaultTopic)
                ? producerConfig!.DefaultTopic
                : $"{consumeResult.Topic}{KafkaRetryConstants.RetryTopicSuffix}";

            try
            {
                await _options.RetryProducer!.ProduceAsync(retryTopic, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogRetryProductionFailure(ex, messageId, retryTopic);
            }
        }

        protected virtual async Task ProduceDeadLetterMessageAsync(ConsumeResult<TKey, TValue> consumeResult, object messageId, Exception exception = null)
        {
            var message = CreateMetadataMessage(consumeResult, messageId, exception);

            var producerConfig = _options.DeadLetterProducer!.Options!.ProducerConfig;

            var deadLetterTopic = !string.IsNullOrWhiteSpace(producerConfig!.DefaultTopic)
                ? producerConfig!.DefaultTopic
                : $"{consumeResult.Topic}{KafkaProducerConstants.DeadLetterTopicSuffix}";

            try
            {
                await _options.DeadLetterProducer!.ProduceAsync(deadLetterTopic, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogDeadLetterProductionFailure(ex, messageId, deadLetterTopic);
            }
        }

        protected virtual Message<byte[], KafkaMetadataMessage> CreateMetadataMessage(ConsumeResult<TKey, TValue> consumeResult, object messageId, Exception exception)
        {
            var key = _options.KeySerializer?.Serialize(
                consumeResult.Message!.Key,
                new SerializationContext(MessageComponentType.Key, consumeResult.Topic, consumeResult.Message!.Headers));

            var value = _options.ValueSerializer?.Serialize(
                consumeResult.Message!.Value,
                new SerializationContext(MessageComponentType.Value, consumeResult.Topic, consumeResult.Message!.Headers));

            var reason = exception is null
                ? null
                : string.Join(Environment.NewLine, exception.GetInnerExceptions().Select(ex => ex.Message));

            var message = new Message<byte[], KafkaMetadataMessage>
            {
                Key = key,
                Value = new KafkaMetadataMessage
                {
                    Id = Guid.NewGuid(),
                    SourceId = messageId,
                    SourceTopic = consumeResult.Topic,
                    SourceGroupId = _options.Consumer!.Options!.ConsumerConfig!.GroupId,
                    SourcePartition = consumeResult.Partition,
                    SourceOffset = consumeResult.Offset,
                    SourceKey = key,
                    SourceValue = value,
                    SourceKeyType = typeof(TKey).AssemblyQualifiedName,
                    SourceValueType = typeof(TValue).AssemblyQualifiedName,
                    ErrorCode = ErrorCode.Unknown,
                    Reason = reason ?? ErrorCode.Unknown.GetReason()
                },
                Headers = consumeResult.Message!.Headers
            };

            return message;
        }

        protected virtual bool ShouldBypassIdempotency(ConsumeResult<TKey, TValue> consumeResult)
        {
            var headers = consumeResult.Message!.Headers?.ToDictionary();

            var hasRetryCountHeader = HasRetryCountHeader(headers);

            var hasRetryGroupIdHeader = HasRetryGroupIdHeader(headers, out string retryGroupId);

            var hasSameGroupId = HasSameGroupId(retryGroupId);

            var shouldBypass = hasRetryCountHeader && hasRetryGroupIdHeader && hasSameGroupId;

            return shouldBypass;
        }

        protected virtual bool ShouldHandleFetchedConsumeResult(ConsumeResult<TKey, TValue> consumeResult)
        {
            var headers = consumeResult.Message!.Headers?.ToDictionary();

            var hasRetryGroupIdHeader = HasRetryGroupIdHeader(headers, out string retryGroupId);

            var hasSameGroupId = HasSameGroupId(retryGroupId);

            var shouldHandle = !hasRetryGroupIdHeader || hasSameGroupId;

            return shouldHandle;
        }

        protected virtual bool HasRetryCountHeader(IDictionary<string, string> headers)
        {
            var hasRetryCountHeader = headers is not null && headers.ContainsKey(KafkaRetryConstants.RetryCountKey);

            return hasRetryCountHeader;
        }

        protected virtual bool HasRetryGroupIdHeader(IDictionary<string, string> headers, out string retryGroupId)
        {
            retryGroupId = null;

            var hasRetryGroupIdHeader = headers is not null && headers.TryGetValue(KafkaRetryConstants.RetryGroupIdKey, out retryGroupId);

            return hasRetryGroupIdHeader;
        }

        protected virtual bool HasSameGroupId(string retryGroupId)
        {
            var hasSameGroupId = string.Equals(_options.Consumer!.Options!.ConsumerConfig!.GroupId, retryGroupId, StringComparison.Ordinal);

            return hasSameGroupId;
        }

        protected virtual Activity StartActivity(string topic, IDictionary<string, string> headers)
        {
            var activityName = $"{topic} {OperationNames.ProcessOperation}";

            var activity = _options.DiagnosticsManager!.StartConsumerActivity(activityName, headers);

            return activity;
        }

        protected virtual TimeSpan GetDelay(bool hasAvailableSlots, bool dispatchedTask)
        {
            TimeSpan delay;

            if (!hasAvailableSlots)
            {
                delay = _options.WorkerConfig!.UnavailableProcessingSlotsDelay;
            }
            else
            {
                if (!dispatchedTask)
                {
                    delay = _options.WorkerConfig!.EmptyTopicDelay;
                }
                else
                {
                    delay = _options.WorkerConfig!.NotEmptyTopicDelay;
                }
            }

            return delay;
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(ServiceName);
        }

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var disposables = new IDisposable[]
                    {
                        _options.Consumer,
                        _options.IdempotencyHandler,
                        _options.RetryProducer,
                        _options.DeadLetterProducer,
                        AsyncLock
                    };

                    foreach (var disposable in disposables)
                    {
                        disposable?.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
