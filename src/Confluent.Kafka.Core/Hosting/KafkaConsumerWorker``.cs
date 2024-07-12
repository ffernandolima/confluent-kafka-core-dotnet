using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Hosting.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Confluent.Kafka.Core.Retry.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Hosting
{
    public sealed class KafkaConsumerWorker<TKey, TValue> : BackgroundService, IKafkaConsumerWorker<TKey, TValue>
    {
        private readonly ILogger _logger;
        private readonly IKafkaConsumerWorkerOptions<TKey, TValue> _options;

        private readonly string _serviceName;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentBag<Exception> _exceptions;
        private readonly ConcurrentQueue<BackgroundWorkItem<TKey, TValue>> _workItems;

        public IKafkaConsumerWorkerOptions<TKey, TValue> Options
        {
            get
            {
                CheckDisposed();
                return _options;
            }
        }

        public KafkaConsumerWorker(IKafkaConsumerWorkerBuilder<TKey, TValue> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var options = builder.ToOptions();

            _logger = options.LoggerFactory.CreateLogger(options.WorkerConfig!.EnableLogging, options.WorkerType);
            _serviceName = options.WorkerType!.ExtractTypeName();
            _semaphore = new SemaphoreSlim(options.WorkerConfig!.MaxDegreeOfParallelism, options.WorkerConfig!.MaxDegreeOfParallelism);
            _exceptions = [];
            _workItems = [];
            _options = options;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            _logger.LogWorkerStarting(_serviceName);

            _options.IdempotencyHandler?.StartAsync(cancellationToken);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            _logger.LogWorkerStopping(_serviceName);

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogWorkerExecuting(_serviceName);

            try
            {
                var dispatched = false;

                var hasAvailableSlots = false;

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await HandleCompletedWorkItemsAsync().ConfigureAwait(false);

                        hasAvailableSlots = HasAvailableSlots();

                        if (hasAvailableSlots)
                        {
                            dispatched = ExecuteInternal(stoppingToken);

                            if (!dispatched)
                            {
                                _logger.LogNoAvailableMessages();
                            }
                        }
                        else
                        {
                            _logger.LogNoAvailableSlots();
                        }

                        HandleConsumptionExceptions();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWorkerExecutionFailure(ex, _serviceName);
                    }

                    var delay = GetDelay(hasAvailableSlots, dispatched);

                    _logger.LogDelayingUntil(DateTime.UtcNow.Add(delay));

                    await Task.Delay(delay).ConfigureAwait(false);
                }
            }
            finally
            {
                var delay = _options.WorkerConfig!.PendingProcessingDelay;

                while (_workItems.Any() || _exceptions.Any())
                {
                    try
                    {
                        await HandleCompletedWorkItemsAsync().ConfigureAwait(false);

                        HandleConsumptionExceptions();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWorkerExecutionFailure(ex, _serviceName);
                    }

                    _logger.LogDelayingUntil(DateTime.UtcNow.Add(delay));

                    await Task.Delay(delay).ConfigureAwait(false);
                }
            }
        }

        private bool HasAvailableSlots() => _semaphore.CurrentCount > 0;

        private bool ExecuteInternal(CancellationToken cancellationToken)
        {
            var hasAvailableSlots = HasAvailableSlots();

            if (!hasAvailableSlots)
            {
                _logger.LogNoAvailableSlots();

                return false;
            }

            IEnumerable<ConsumeResult<TKey, TValue>> consumeResults = null;

            try
            {
                consumeResults = _options.Consumer!.ConsumeBatch(_semaphore.CurrentCount);
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
            }

            var dispatched = false;

            if (consumeResults is not null && consumeResults.Any())
            {
                foreach (var consumeResult in consumeResults.Where(consumeResult => consumeResult!.Message is not null))
                {
                    DispatchWorkItem(consumeResult, cancellationToken);

                    dispatched = true;
                }
            }

            return dispatched;
        }

        private async Task HandleCompletedWorkItemsAsync()
        {
            if (!_workItems.Any())
            {
                return;
            }

            try
            {
                while (_workItems.TryPeek(out BackgroundWorkItem<TKey, TValue> workItem))
                {
                    if (!workItem.IsCompleted || !_workItems.TryDequeue(out workItem))
                    {
                        break;
                    }

                    if (!workItem.IsFaulted && !workItem.IsCanceled)
                    {
                        Commit(workItem.ConsumeResult);

                        StoreOffset(workItem.ConsumeResult);

                        if (!workItem.IsHandled)
                        {
                            HandleCompletedWorkItem(workItem);
                        }
                    }
                    else
                    {
                        if (_options.WorkerConfig!.CommitFaultedMessages)
                        {
                            Commit(workItem.ConsumeResult);

                            StoreOffset(workItem.ConsumeResult);
                        }

                        if (!workItem.IsHandled)
                        {
                            await HandleFaultedWorkItem(workItem).ConfigureAwait(false);
                        }
                    }
                }

                var enumerator = _workItems.GetEnumerator();

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

                void Commit(ConsumeResult<TKey, TValue> consumeResult)
                {
                    try
                    {
                        _options.Consumer!.Commit(consumeResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogMessageCommitFailure(ex, messageId);
                    }
                }

                void StoreOffset(ConsumeResult<TKey, TValue> consumeResult)
                {
                    try
                    {
                        if (!_options.Consumer!.Options!.ConsumerConfig!.EnableAutoOffsetStore.GetValueOrDefault(defaultValue: true))
                        {
                            _options.Consumer!.StoreOffset(consumeResult);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogMessageOffsetStorageFailure(ex, messageId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCompletionHandlingFailure(ex);
            }
        }

        private void HandleConsumptionExceptions()
        {
            if (!_exceptions.Any())
            {
                return;
            }

            try
            {
                while (_exceptions.TryTake(out Exception exception))
                {
                    _logger.LogMessageConsumptionFailure(exception);
                }
            }
            catch (Exception ex)
            {
                _logger.LogExceptionHandlingFailure(ex);
            }
        }

        private void DispatchWorkItem(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken)
        {
            TaskActivity taskActivity = null;

            var messageId = consumeResult.Message!.GetId(_options.Consumer!.Options!.MessageIdHandler);

            var headers = consumeResult.Message!.Headers?.ToDictionary();

            try
            {
                taskActivity = TaskActivity.Run(async activity =>
                {
                    var stopwatch = Stopwatch.StartNew();

                    activity = StartActivity(consumeResult.Topic, headers);

                    await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                    _logger.LogCurrentThreadBlocked(messageId, stopwatch.Elapsed);

                    try
                    {
                        if (_options.WorkerConfig!.EnableIdempotency)
                        {
                            _logger.LogIdempotencyEnabled(messageId);

                            if (ShouldBypassIdempotency(consumeResult))
                            {
                                _logger.LogIdempotencyBypassed(messageId);
                            }
                            else if (!await _options.IdempotencyHandler!.TryHandleAsync(consumeResult.Message!.Value, cancellationToken)
                                .ConfigureAwait(false))
                            {
                                _logger.LogMessageAlreadyProcessed(messageId);

                                return;
                            }
                        }
                        else
                        {
                            _logger.LogIdempotencyDisabled(messageId);

                            if (!ShouldHandleFetchedConsumeResult(consumeResult))
                            {
                                _logger.LogMessageProcessingSkip(messageId);

                                return;
                            }
                        }

                        if (_options.WorkerConfig!.EnableRetryOnFailure)
                        {
                            _logger.LogRetryStrategyEnabled(messageId);

                            await _options.RetryHandler!.HandleAsync(
                                executeAction: async cancellationToken =>
                                    await HandleFetchedConsumeResultAsync(consumeResult, cancellationToken).ConfigureAwait(false),
                                cancellationToken: cancellationToken,
                                onRetryAction: (exception, timeSpan, retryAttempt) =>
                                    _logger.LogMessageProcessingRetryFailure(exception, messageId, retryAttempt))
                            .ConfigureAwait(false);
                        }
                        else
                        {
                            _logger.LogRetryStrategyDisabled(messageId);

                            await HandleFetchedConsumeResultAsync(consumeResult, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
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
                    _logger.LogWorkItemDispatched(
                        messageId,
                        consumeResult.Topic,
                        consumeResult.Partition,
                        consumeResult.Offset,
                        consumeResult.Message!.Value,
                        headers);

                    var workItem = new BackgroundWorkItem<TKey, TValue>(taskActivity, consumeResult)
                        .AttachContinuation(taskActivity => taskActivity.TraceActivity?.SetEndTime(DateTime.UtcNow));

                    _workItems.Enqueue(workItem);
                }
            }
        }

        private void HandleCompletedWorkItem(BackgroundWorkItem<TKey, TValue> workItem)
        {
            var consumeResult = workItem.ConsumeResult;

            var activity = workItem.TaskActivity!.TraceActivity;

            var messageId = consumeResult!.Message!.GetId(_options.Consumer!.Options!.MessageIdHandler);

            try
            {
                _logger.LogMessageProcessingSuccess(messageId);

                _options.DiagnosticsManager!.Enrich(activity, consumeResult, _options);
            }
            finally
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
                activity?.Dispose();

                workItem.SetHandled();
            }
        }

        private async Task HandleFaultedWorkItem(BackgroundWorkItem<TKey, TValue> workItem)
        {
            var consumeResult = workItem.ConsumeResult;

            var activity = workItem.TaskActivity!.TraceActivity;

            var exception = await workItem.GetExceptionAsync().ConfigureAwait(false);

            var messageId = consumeResult!.Message!.GetId(_options.Consumer!.Options!.MessageIdHandler);

            try
            {
                _logger.LogMessageProcessingFailure(exception, messageId);

                _options.DiagnosticsManager!.Enrich(activity, consumeResult, _options, exception);

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
                        _logger.LogErrorHandlerFailure(ex, messageId);
                    }
                }
            }
        }

        private bool ShouldProduceRetryMessage(Exception exception, object messageId)
        {
            var shouldProduce = _options.WorkerConfig!.RetryTopicSpecification!.IsSatisfiedBy(exception);

            if (!shouldProduce)
            {
                _logger.LogMessageProcessingNotRetriable(messageId);
            }

            return shouldProduce;
        }

        private async Task ProduceRetryMessageAsync(ConsumeResult<TKey, TValue> consumeResult, object messageId, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(consumeResult?.Topic))
            {
                return;
            }

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
                _logger.LogRetryProductionFailure(ex, messageId, retryTopic);
            }
        }

        private async Task ProduceDeadLetterMessageAsync(ConsumeResult<TKey, TValue> consumeResult, object messageId, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(consumeResult?.Topic))
            {
                return;
            }

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
                _logger.LogDeadLetterProductionFailure(ex, messageId, deadLetterTopic);
            }
        }

        private Message<byte[], KafkaMetadataMessage> CreateMetadataMessage(ConsumeResult<TKey, TValue> consumeResult, object messageId, Exception exception)
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
                    SourceMessage = value,
                    SourceKeyType = typeof(TKey).AssemblyQualifiedName,
                    SourceMessageType = typeof(TValue).AssemblyQualifiedName,
                    ErrorCode = ErrorCode.Unknown,
                    Reason = reason ?? ErrorCode.Unknown.GetReason()
                },
                Headers = consumeResult.Message!.Headers
            };

            return message;
        }

        private bool ShouldBypassIdempotency(ConsumeResult<TKey, TValue> consumeResult)
        {
            var headers = consumeResult.Message!.Headers?.ToDictionary();

            var hasRetryCountHeader = HasRetryCountHeader(headers);

            var hasRetryGroupIdHeader = HasRetryGroupIdHeader(headers, out string retryGroupId);

            var hasSameGroupId = HasSameGroupId(retryGroupId);

            var shouldBypass = hasRetryCountHeader && hasRetryGroupIdHeader && hasSameGroupId;

            return shouldBypass;
        }

        private bool ShouldHandleFetchedConsumeResult(ConsumeResult<TKey, TValue> consumeResult)
        {
            var headers = consumeResult.Message!.Headers?.ToDictionary();

            var hasRetryGroupIdHeader = HasRetryGroupIdHeader(headers, out string retryGroupId);

            var hasSameGroupId = HasSameGroupId(retryGroupId);

            var shouldHandle = !hasRetryGroupIdHeader || hasSameGroupId;

            return shouldHandle;
        }

        private bool HasRetryCountHeader(IDictionary<string, string> headers)
        {
            var hasRetryCountHeader = headers is not null && headers.ContainsKey(KafkaRetryConstants.RetryCountKey);

            return hasRetryCountHeader;
        }

        private bool HasRetryGroupIdHeader(IDictionary<string, string> headers, out string retryGroupId)
        {
            retryGroupId = null;

            var hasRetryGroupIdHeader = headers is not null && headers.TryGetValue(KafkaRetryConstants.RetryGroupIdKey, out retryGroupId);

            return hasRetryGroupIdHeader;
        }

        private bool HasSameGroupId(string retryGroupId)
        {
            var hasSameGroupId = string.Equals(_options.Consumer!.Options!.ConsumerConfig!.GroupId, retryGroupId, StringComparison.Ordinal);

            return hasSameGroupId;
        }

        private Activity StartActivity(string topic, IDictionary<string, string> headers)
        {
            var activityName = $"{topic} {OperationNames.ProcessOperation}";

            var activity = _options.DiagnosticsManager!.StartConsumerActivity(activityName, headers);

            return activity;
        }

        private TimeSpan GetDelay(bool hasAvailableSlots, bool dispatchedTask)
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

            throw new ObjectDisposedException(_serviceName);
        }

        #region IDisposable Members

        private bool _disposed;

        public override void Dispose()
        {
            _disposed = true;

            base.Dispose();

            var disposables = new IDisposable[]
            {
                _options.Consumer,
                _options.IdempotencyHandler,
                _options.RetryProducer,
                _options.DeadLetterProducer,
                _semaphore
            };

            foreach (var disposable in disposables)
            {
                disposable?.Dispose();
            }
        }

        #endregion IDisposable Members
    }
}
