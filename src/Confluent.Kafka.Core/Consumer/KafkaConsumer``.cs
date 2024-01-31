﻿using Confluent.Kafka.Core.Consumer.Internal;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Producer.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Confluent.Kafka.Core.Consumer
{
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>, IConsumerAccessor<TKey, TValue> // TODO: Make this internal sealed?
    {
        private readonly ILogger _logger;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IConsumer<TKey, TValue> _consumer;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IKafkaConsumerOptions<TKey, TValue> _options;

        public Handle Handle => _consumer.Handle;
        public string Name => _consumer.Name;
        public string MemberId => _consumer.MemberId;
        public List<string> Subscription => _consumer.Subscription;
        public List<TopicPartition> Assignment => _consumer.Assignment;
        public IConsumerGroupMetadata ConsumerGroupMetadata => _consumer.ConsumerGroupMetadata;
        public IKafkaConsumerOptions<TKey, TValue> Options => _options;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConsumer<TKey, TValue> IConsumerAccessor<TKey, TValue>.UnderlyingConsumer => _consumer;

        public KafkaConsumer(IKafkaConsumerBuilder<TKey, TValue> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder), $"{nameof(builder)} cannot be null.");
            }

            var options = builder.ToOptions();

            _logger = options.LoggerFactory.CreateLogger(options.ConsumerConfig!.EnableLogging, options.ConsumerType);
            _consumer = builder.BuildUnderlyingConsumer();
            _options = options;
        }

        public int AddBrokers(string brokers)
        {
            if (string.IsNullOrWhiteSpace(brokers))
            {
                throw new ArgumentException($"{nameof(brokers)} cannot be null or whitespace.", nameof(brokers));
            }

            var brokersResult = _consumer.AddBrokers(brokers);

            return brokersResult;
        }

        public void SetSaslCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException($"{nameof(username)} cannot be null or whitespace.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"{nameof(password)} cannot be null or whitespace.", nameof(password));
            }

            _consumer.SetSaslCredentials(username, password);
        }

        public ConsumeResult<TKey, TValue> Consume()
        {
            var consumeResult = Consume(_options.ConsumerConfig!.DefaultTimeout);

            return consumeResult;
        }

        public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
        {
            if (millisecondsTimeout == Timeout.Infinite)
            {
                throw new ArgumentException($"{nameof(millisecondsTimeout)} cannot be infinite.", nameof(millisecondsTimeout));
            }

            ConsumeResult<TKey, TValue> consumeResult = null;

            try
            {
                if (!_options.ConsumerConfig!.EnableRetryOnFailure)
                {
                    consumeResult = ConsumeInternal(millisecondsTimeout);
                }
                else
                {
                    _options.RetryHandler!.TryHandle(
                        executeAction: _ => consumeResult = ConsumeInternal(millisecondsTimeout),
                        onRetryAction: (exception, _, retryAttempt) => _logger.LogMessageConsumptionRetryFailure(exception, retryAttempt));
                }
            }
            catch (ConsumeException ex)
            {
                ProduceDeadLetterMessage(ex.ConsumerRecord, ex.Error);
                throw;
            }

            return consumeResult;
        }

        public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            ConsumeResult<TKey, TValue> consumeResult = null;

            try
            {
                if (!_options.ConsumerConfig!.EnableRetryOnFailure)
                {
                    consumeResult = ConsumeInternal(timeout);
                }
                else
                {
                    _options.RetryHandler!.TryHandle(
                        executeAction: _ => consumeResult = ConsumeInternal(timeout),
                        onRetryAction: (exception, _, retryAttempt) => _logger.LogMessageConsumptionRetryFailure(exception, retryAttempt));
                }
            }
            catch (ConsumeException ex)
            {
                ProduceDeadLetterMessage(ex.ConsumerRecord, ex.Error);
                throw;
            }

            return consumeResult;
        }

        public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                throw new ArgumentException($"{nameof(cancellationToken)} should be capable of being canceled.", nameof(cancellationToken));
            }

            ConsumeResult<TKey, TValue> consumeResult = null;

            try
            {
                if (!_options.ConsumerConfig!.EnableRetryOnFailure)
                {
                    consumeResult = ConsumeInternal(cancellationToken);
                }
                else
                {
                    _options.RetryHandler!.TryHandle(
                        executeAction: cancellationToken => consumeResult = ConsumeInternal(cancellationToken),
                        cancellationToken: cancellationToken,
                        onRetryAction: (exception, _, retryAttempt) => _logger.LogMessageConsumptionRetryFailure(exception, retryAttempt));
                }
            }
            catch (ConsumeException ex)
            {
                ProduceDeadLetterMessage(ex.ConsumerRecord, ex.Error);
                throw;
            }

            return consumeResult;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch()
        {
            var consumeResults = ConsumeBatch(_options.ConsumerConfig!.DefaultBatchSize, _options.ConsumerConfig!.DefaultTimeout);

            return consumeResults;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(int batchSize)
        {
            var consumeResults = ConsumeBatch(batchSize, _options.ConsumerConfig!.DefaultTimeout);

            return consumeResults;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(TimeSpan timeout)
        {
            var consumeResults = ConsumeBatch(_options.ConsumerConfig!.DefaultBatchSize, timeout);

            return consumeResults;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(int batchSize, TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var tokenSource = new CancellationTokenSource(timeout);

            var consumeResults = ConsumeBatch(batchSize, tokenSource.Token);

            return consumeResults;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(CancellationToken cancellationToken)
        {
            var consumeResults = ConsumeBatch(_options.ConsumerConfig!.DefaultBatchSize, cancellationToken);

            return consumeResults;
        }

        public IEnumerable<ConsumeResult<TKey, TValue>> ConsumeBatch(int batchSize, CancellationToken cancellationToken)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentException($"{nameof(batchSize)} cannot be less than or equal to zero.", $"{nameof(batchSize)}");
            }

            if (!cancellationToken.CanBeCanceled)
            {
                throw new ArgumentException($"{nameof(cancellationToken)} should be capable of being canceled.", nameof(cancellationToken));
            }

            var consumeResults = new List<ConsumeResult<TKey, TValue>>();

            while (consumeResults.Count < batchSize && !cancellationToken.IsCancellationRequested)
            {
                var consumeResult = Consume(_options.ConsumerConfig!.CancellationDelayMaxMs);

                if (consumeResult is not null)
                {
                    consumeResults.Add(consumeResult);
                }
            }

            return consumeResults.AsReadOnly();
        }

        public void Subscribe(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException($"{nameof(topic)} cannot be null or whitespace.", nameof(topic));
            }

            _consumer.Subscribe(topic);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics is null || !topics.Any(topic => !string.IsNullOrWhiteSpace(topic)))
            {
                throw new ArgumentException($"{nameof(topics)} cannot be null, empty, or contain null values.", nameof(topics));
            }

            var subscriptions = topics.Where(topic => !string.IsNullOrWhiteSpace(topic)).Distinct(StringComparer.Ordinal);

            _consumer.Subscribe(subscriptions);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void Unsubscribe()
        {
            _consumer.Unsubscribe();

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void Assign(TopicPartition partition)
        {
            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition), $"{nameof(partition)} cannot be null.");
            }

            _consumer.Assign(partition);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void Assign(TopicPartitionOffset offset)
        {
            if (offset is null)
            {
                throw new ArgumentNullException(nameof(offset), $"{nameof(offset)} cannot be null.");
            }

            _consumer.Assign(offset);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void Assign(IEnumerable<TopicPartition> partitions)
        {
            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var assignments = partitions.Where(partition => partition is not null).Distinct();

            _consumer.Assign(assignments);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void Assign(IEnumerable<TopicPartitionOffset> offsets)
        {
            if (offsets is null || !offsets.Any(offset => offset is not null))
            {
                throw new ArgumentException($"{nameof(offsets)} cannot be null, empty, or contain null values.", nameof(offsets));
            }

            var assignments = offsets.Where(offset => offset is not null).Distinct();

            _consumer.Assign(assignments);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void IncrementalAssign(IEnumerable<TopicPartition> partitions)
        {
            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var assignments = partitions.Where(partition => partition is not null).Distinct();

            _consumer.IncrementalAssign(assignments);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void IncrementalAssign(IEnumerable<TopicPartitionOffset> offsets)
        {
            if (offsets is null || !offsets.Any(offset => offset is not null))
            {
                throw new ArgumentException($"{nameof(offsets)} cannot be null, empty, or contain null values.", nameof(offsets));
            }

            var assignments = offsets.Where(offset => offset is not null).Distinct();

            _consumer.IncrementalAssign(assignments);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void IncrementalUnassign(IEnumerable<TopicPartition> partitions)
        {
            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var unassignments = partitions.Where(partition => partition is not null).Distinct();

            _consumer.IncrementalUnassign(unassignments);

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void Unassign()
        {
            _consumer.Unassign();

            OnSubscriptionsOrAssignmentsChanged();
        }

        public void StoreOffset(ConsumeResult<TKey, TValue> consumeResult)
        {
            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult), $"{nameof(consumeResult)} cannot be null.");
            }

            _consumer.StoreOffset(consumeResult);
        }

        public void StoreOffset(TopicPartitionOffset offset)
        {
            if (offset is null)
            {
                throw new ArgumentNullException(nameof(offset), $"{nameof(offset)} cannot be null.");
            }

            _consumer.StoreOffset(offset);
        }

        public List<TopicPartitionOffset> Commit()
        {
            var offsets = _consumer.Commit();

            return offsets;
        }

        public void Commit(IEnumerable<TopicPartitionOffset> offsets)
        {
            if (offsets is null || !offsets.Any(offset => offset is not null))
            {
                throw new ArgumentException($"{nameof(offsets)} cannot be null, empty, or contain null values.", nameof(offsets));
            }

            var comparer = Comparer<TopicPartitionOffset>.Create((left, right) =>
            {
                var result = left.TopicPartition.CompareTo(right.TopicPartition);

                if (result == 0)
                {
                    result = left.Offset.Value.CompareTo(right.Offset.Value);
                }

                return result;
            });

            var commitments = offsets.Where(offset => offset is not null).Distinct().OrderBy(offset => offset, comparer);

            _consumer.Commit(commitments);
        }

        public void Commit(ConsumeResult<TKey, TValue> consumeResult)
        {
            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult), $"{nameof(consumeResult)} cannot be null.");
            }

            _consumer.Commit(consumeResult);
        }

        public List<TopicPartitionOffset> Committed(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var committedOffsets = _consumer.Committed(timeout);

            return committedOffsets;
        }

        public List<TopicPartitionOffset> Committed(IEnumerable<TopicPartition> partitions, TimeSpan timeout)
        {
            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var requestedPartitions = partitions.Where(partition => partition is not null).Distinct();

            var committedOffsets = _consumer.Committed(requestedPartitions, timeout);

            return committedOffsets;
        }

        public void Seek(TopicPartitionOffset offset)
        {
            if (offset is null)
            {
                throw new ArgumentNullException(nameof(offset), $"{nameof(offset)} cannot be null.");
            }

            _consumer.Seek(offset);
        }

        public IEnumerable<TopicPartitionOffset> SeekBatch(IEnumerable<ConsumeResult<TKey, TValue>> consumeResults)
        {
            if (consumeResults is null || !consumeResults.Any(result => result is not null))
            {
                throw new ArgumentException($"{nameof(consumeResults)} cannot be null, empty, or contain null values.", nameof(consumeResults));
            }

            var offsets = consumeResults.Where(result => result is not null)
                                        .GroupBy(result => result.TopicPartition)
                                        .Select(sublist => new TopicPartitionOffset(
                                            sublist.Key, new Offset(sublist.Min(result => result.Offset))));

            foreach (var offset in offsets)
            {
                _consumer.Seek(offset);
            }

            return offsets;
        }

        public void Pause(IEnumerable<TopicPartition> partitions)
        {
            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var pausingPartitions = partitions.Where(partition => partition is not null).Distinct();

            _consumer.Pause(pausingPartitions);
        }

        public void Resume(IEnumerable<TopicPartition> partitions)
        {
            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty or contain null values.", nameof(partitions));
            }

            var resumingPartitions = partitions.Where(partition => partition is not null).Distinct();

            _consumer.Resume(resumingPartitions);
        }

        public Offset Position(TopicPartition partition)
        {
            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition), $"{nameof(partition)} cannot be null.");
            }

            var offsetPosition = _consumer.Position(partition);

            return offsetPosition;
        }

        public List<TopicPartitionOffset> OffsetsForTimes(IEnumerable<TopicPartitionTimestamp> timestamps, TimeSpan timeout)
        {
            if (timestamps is null || !timestamps.Any(timestamp => timestamp is not null))
            {
                throw new ArgumentException($"{nameof(timestamps)} cannot be null, empty, or contain null values.", nameof(timestamps));
            }

            var lookupTimestamps = timestamps.Where(timestamp => timestamp is not null).Distinct();

            var earliestOffsets = _consumer.OffsetsForTimes(lookupTimestamps, timeout);

            return earliestOffsets;
        }

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition partition)
        {
            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition), $"{nameof(partition)} cannot be null.");
            }

            var watermarkOffsets = _consumer.GetWatermarkOffsets(partition);

            return watermarkOffsets;
        }

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition partition, TimeSpan timeout)
        {
            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition), $"{nameof(partition)} cannot be null.");
            }

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var watermarkOffsets = _consumer.QueryWatermarkOffsets(partition, timeout);

            return watermarkOffsets;
        }

        public IEnumerable<KafkaTopicPartitionLag> Lag()
        {
            var lags = _consumer.Assignment!.Select(
                assignment =>
                {
                    var watermarkOffsets = _consumer.GetWatermarkOffsets(assignment);

                    var highOffset = watermarkOffsets?.High ?? Offset.Unset;

                    if (highOffset == Offset.Unset)
                    {
                        highOffset = 0;
                    }

                    var currentOffset = _consumer.Position(assignment);

                    if (currentOffset == Offset.Unset)
                    {
                        currentOffset = 0;
                    }

                    var lag = new KafkaTopicPartitionLag(
                        assignment.Topic,
                        assignment.Partition,
                        highOffset - currentOffset);

                    return lag;
                })
            .ToList()
            .AsReadOnly();

            return lags;
        }

        public void Close()
        {
            _consumer.Close();
        }

        private void OnSubscriptionsOrAssignmentsChanged()
        {
            _options.ConsumerConfig!.OnSubscriptionsOrAssignmentsChanged(_consumer.Subscription, _consumer.Assignment);
        }

        private ConsumeResult<TKey, TValue> ConsumeInternal(int millisecondsTimeout)
        {
            ConsumeResult<TKey, TValue> consumeResult;

            try
            {
                consumeResult = _consumer.Consume(millisecondsTimeout);

                consumeResult = PostConsumeInternal(consumeResult);
            }
            catch (ConsumeException ex)
            {
                HandleConsumeException(ex);

                throw;
            }

            return consumeResult;
        }

        private ConsumeResult<TKey, TValue> ConsumeInternal(TimeSpan timeout)
        {
            ConsumeResult<TKey, TValue> consumeResult;

            try
            {
                consumeResult = _consumer.Consume(timeout);

                consumeResult = PostConsumeInternal(consumeResult);
            }
            catch (ConsumeException ex)
            {
                HandleConsumeException(ex);

                throw;
            }

            return consumeResult;
        }

        private ConsumeResult<TKey, TValue> ConsumeInternal(CancellationToken cancellationToken)
        {
            ConsumeResult<TKey, TValue> consumeResult;

            try
            {
                consumeResult = _consumer.Consume(cancellationToken);

                consumeResult = PostConsumeInternal(consumeResult);
            }
            catch (ConsumeException ex)
            {
                HandleConsumeException(ex);

                throw;
            }

            return consumeResult;
        }

        private ConsumeResult<TKey, TValue> PostConsumeInternal(ConsumeResult<TKey, TValue> consumeResult)
        {
            if (consumeResult is null)
            {
                return null;
            }

            Intercept(consumeResult);

            if (consumeResult.IsPartitionEOF)
            {
                _logger.LogEndOfTopicPartitionReached(consumeResult.Topic, consumeResult.Partition);
            }
            else
            {
                using var activity = StartActivity(consumeResult.Topic, consumeResult.Message!.Headers?.ToDictionary());

                if (_options.ConsumerConfig!.CommitAfterConsuming)
                {
                    try
                    {
                        _consumer.Commit(consumeResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogMessageCommitmentFailure(ex, consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
                    }
                }

                var messageId = _options.MessageIdHandler?.Invoke(consumeResult.Message!.Value);

                _logger.LogMessageConsumptionSuccess(messageId);

                activity?.SetStatus(ActivityStatusCode.Ok);

                _options.DiagnosticsManager!.Enrich(activity, consumeResult, _options);
            }

            return consumeResult;
        }

        private void Intercept(ConsumeResult<TKey, TValue> consumeResult)
        {
            if (!_options.Interceptors!.Any())
            {
                return;
            }

            var context = new KafkaConsumerInterceptorContext<TKey, TValue>
            {
                ConsumeResult = consumeResult,
                ConsumerConfig = _options.ConsumerConfig
            };

            foreach (var interceptor in _options.Interceptors)
            {
                try
                {
                    interceptor.OnConsume(context);
                }
                catch (Exception ex)
                {
                    _logger.LogMessageConsumptionInterceptionFailure(
                        ex,
                        consumeResult.Topic,
                        consumeResult.Partition,
                        consumeResult.Offset);

                    if (_options.ConsumerConfig!.EnableInterceptorExceptionPropagation)
                    {
                        throw;
                    }
                }
            }
        }

        private void HandleConsumeException(ConsumeException consumeException)
        {
            using var activity = StartActivity(
                consumeException.ConsumerRecord!.Topic,
                consumeException.ConsumerRecord!.Message!.Headers?.ToDictionary());

            _logger.LogMessageConsumptionFailure(
                consumeException,
                consumeException.ConsumerRecord!.Topic,
                consumeException.ConsumerRecord!.Partition,
                consumeException.ConsumerRecord!.Offset);

            activity?.SetStatus(ActivityStatusCode.Error);

            _options.DiagnosticsManager!.Enrich(activity, consumeException, _options.ConsumerConfig);
        }

        private void ProduceDeadLetterMessage(ConsumeResult<byte[], byte[]> consumeResult, Error error)
        {
            if (_options.ConsumerConfig!.EnableDeadLetterTopic && consumeResult is not null)
            {
                var producerConfig = _options.DeadLetterProducer!.Options!.ProducerConfig;

                var deadLetterTopic = !string.IsNullOrWhiteSpace(producerConfig!.DefaultTopic)
                    ? producerConfig!.DefaultTopic
                    : $"{consumeResult.Topic}{KafkaProducerConstants.DeadLetterTopicSuffix}";

                var message = new Message<byte[], KafkaMetadataMessage>
                {
                    Key = consumeResult.Message?.Key,
                    Value = new KafkaMetadataMessage
                    {
                        Id = Guid.NewGuid(),
                        SourceTopic = consumeResult.Topic,
                        SourceGroupId = _options.ConsumerConfig!.GroupId,
                        SourcePartition = consumeResult.Partition,
                        SourceOffset = consumeResult.Offset,
                        SourceKey = consumeResult.Message?.Key,
                        SourceMessage = consumeResult.Message?.Value,
                        SourceKeyType = typeof(TKey).AssemblyQualifiedName,
                        SourceMessageType = typeof(TValue).AssemblyQualifiedName,
                        ErrorCode = error?.Code ?? ErrorCode.Unknown,
                        Reason = error?.Reason ?? ErrorCode.Unknown.GetReason()
                    },
                    Headers = consumeResult.Message?.Headers
                };

                try
                {
                    _options.DeadLetterProducer!.Produce(deadLetterTopic, message);
                }
                catch (Exception ex)
                {
                    _logger.LogDeadLetterProductionFailure(ex, deadLetterTopic);
                }

                try
                {
                    _consumer.Commit(new[] { new TopicPartitionOffset(consumeResult.TopicPartition, consumeResult.Offset + 1) });
                }
                catch (Exception ex)
                {
                    _logger.LogMessageCommitmentFailure(ex, consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
                }
            }
        }

        private Activity StartActivity(string topic, IDictionary<string, string> headers)
        {
            var activityName = $"{topic} {OperationNames.ReceiveOperation}";

            var activity = _options.DiagnosticsManager!.StartConsumerActivity(activityName, headers);

            return activity;
        }

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _consumer?.Close();
                    _consumer?.Dispose();
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