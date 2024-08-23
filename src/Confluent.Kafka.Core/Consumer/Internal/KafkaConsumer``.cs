using Confluent.Kafka.Core.Conversion.Internal;
using Confluent.Kafka.Core.Diagnostics.Internal;
using Confluent.Kafka.Core.Internal;
using Confluent.Kafka.Core.Models;
using Confluent.Kafka.Core.Models.Internal;
using Confluent.Kafka.Core.Producer.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Confluent.Kafka.Core.Consumer.Internal
{
    internal sealed class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>, IConsumerAccessor<TKey, TValue>
    {
        private readonly ILogger _logger;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IConsumer<TKey, TValue> _consumer;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IKafkaConsumerOptions<TKey, TValue> _options;

        public Handle Handle
        {
            get
            {
                CheckDisposed();
                return _consumer.Handle;
            }
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                return _consumer.Name;
            }
        }

        public string MemberId
        {
            get
            {
                CheckDisposed();
                return _consumer.MemberId;
            }
        }

        public List<string> Subscription
        {
            get
            {
                CheckDisposed();
                return _consumer.Subscription;
            }
        }

        public List<TopicPartition> Assignment
        {
            get
            {
                CheckDisposed();
                return _consumer.Assignment;
            }
        }

        public IConsumerGroupMetadata ConsumerGroupMetadata
        {
            get
            {
                CheckDisposed();
                return _consumer.ConsumerGroupMetadata;
            }
        }

        public IKafkaConsumerOptions<TKey, TValue> Options
        {
            get
            {
                CheckDisposed();
                return _options;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConsumer<TKey, TValue> IConsumerAccessor<TKey, TValue>.UnderlyingConsumer
        {
            get
            {
                CheckDisposed();
                return _consumer;
            }
        }

        public KafkaConsumer(IKafkaConsumerBuilder<TKey, TValue> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var options = builder.ToOptions<IKafkaConsumerOptions<TKey, TValue>>();

            _logger = options.LoggerFactory.CreateLogger(options.ConsumerConfig!.EnableLogging, options.ConsumerType);
            _consumer = builder.BuildUnderlyingConsumer();
            _options = options;
        }

        public int AddBrokers(string brokers)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(brokers))
            {
                throw new ArgumentException($"{nameof(brokers)} cannot be null or whitespace.", nameof(brokers));
            }

            var brokersResult = _consumer.AddBrokers(brokers);

            return brokersResult;
        }

        public void SetSaslCredentials(string username, string password)
        {
            CheckDisposed();

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
            CheckDisposed();

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
                    _options.RetryHandler!.Handle(
                        executeAction: _ => consumeResult = ConsumeInternal(millisecondsTimeout),
                        onRetryAction: (exception, _, retryAttempt) => OnConsumeRetry(exception, retryAttempt));
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
            CheckDisposed();

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
                    _options.RetryHandler!.Handle(
                        executeAction: _ => consumeResult = ConsumeInternal(timeout),
                        onRetryAction: (exception, _, retryAttempt) => OnConsumeRetry(exception, retryAttempt));
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
            CheckDisposed();

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
                    _options.RetryHandler!.Handle(
                        executeAction: cancellationToken => consumeResult = ConsumeInternal(cancellationToken),
                        cancellationToken: cancellationToken,
                        onRetryAction: (exception, _, retryAttempt) => OnConsumeRetry(exception, retryAttempt));
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
            CheckDisposed();

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
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException($"{nameof(topic)} cannot be null or whitespace.", nameof(topic));
            }

            _consumer.Subscribe(topic);
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            CheckDisposed();

            if (topics is null || !topics.Any(topic => !string.IsNullOrWhiteSpace(topic)))
            {
                throw new ArgumentException($"{nameof(topics)} cannot be null, empty, or contain null values.", nameof(topics));
            }

            var subscriptions = topics.Where(topic => !string.IsNullOrWhiteSpace(topic)).Distinct(StringComparer.Ordinal);

            _consumer.Subscribe(subscriptions);
        }

        public void Unsubscribe()
        {
            CheckDisposed();

            _consumer.Unsubscribe();
        }

        public void Assign(TopicPartition partition)
        {
            CheckDisposed();

            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition));
            }

            _consumer.Assign(partition);
        }

        public void Assign(TopicPartitionOffset offset)
        {
            CheckDisposed();

            if (offset is null)
            {
                throw new ArgumentNullException(nameof(offset));
            }

            _consumer.Assign(offset);
        }

        public void Assign(IEnumerable<TopicPartition> partitions)
        {
            CheckDisposed();

            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var assignments = partitions.Where(partition => partition is not null).Distinct();

            _consumer.Assign(assignments);
        }

        public void Assign(IEnumerable<TopicPartitionOffset> offsets)
        {
            CheckDisposed();

            if (offsets is null || !offsets.Any(offset => offset is not null))
            {
                throw new ArgumentException($"{nameof(offsets)} cannot be null, empty, or contain null values.", nameof(offsets));
            }

            var assignments = offsets.Where(offset => offset is not null).Distinct();

            _consumer.Assign(assignments);
        }

        public void IncrementalAssign(IEnumerable<TopicPartition> partitions)
        {
            CheckDisposed();

            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var assignments = partitions.Where(partition => partition is not null).Distinct();

            _consumer.IncrementalAssign(assignments);
        }

        public void IncrementalAssign(IEnumerable<TopicPartitionOffset> offsets)
        {
            CheckDisposed();

            if (offsets is null || !offsets.Any(offset => offset is not null))
            {
                throw new ArgumentException($"{nameof(offsets)} cannot be null, empty, or contain null values.", nameof(offsets));
            }

            var assignments = offsets.Where(offset => offset is not null).Distinct();

            _consumer.IncrementalAssign(assignments);
        }

        public void IncrementalUnassign(IEnumerable<TopicPartition> partitions)
        {
            CheckDisposed();

            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var unassignments = partitions.Where(partition => partition is not null).Distinct();

            _consumer.IncrementalUnassign(unassignments);
        }

        public void Unassign()
        {
            CheckDisposed();

            _consumer.Unassign();
        }

        public void StoreOffset(ConsumeResult<TKey, TValue> consumeResult)
        {
            CheckDisposed();

            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult));
            }

            _consumer.StoreOffset(consumeResult);
        }

        public void StoreOffset(TopicPartitionOffset offset)
        {
            CheckDisposed();

            if (offset is null)
            {
                throw new ArgumentNullException(nameof(offset));
            }

            _consumer.StoreOffset(offset);
        }

        public List<TopicPartitionOffset> Commit()
        {
            CheckDisposed();

            var offsets = _consumer.Commit();

            return offsets;
        }

        public void Commit(IEnumerable<TopicPartitionOffset> offsets)
        {
            CheckDisposed();

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

            var commitOffsets = offsets.Where(offset => offset is not null).Distinct().OrderBy(offset => offset, comparer);

            _consumer.Commit(commitOffsets);
        }

        public void Commit(ConsumeResult<TKey, TValue> consumeResult)
        {
            CheckDisposed();

            if (consumeResult is null)
            {
                throw new ArgumentNullException(nameof(consumeResult));
            }

            _consumer.Commit(consumeResult);
        }

        public List<TopicPartitionOffset> Committed(TimeSpan timeout)
        {
            CheckDisposed();

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException($"{nameof(timeout)} cannot be infinite.", nameof(timeout));
            }

            var committedOffsets = _consumer.Committed(timeout);

            return committedOffsets;
        }

        public List<TopicPartitionOffset> Committed(IEnumerable<TopicPartition> partitions, TimeSpan timeout)
        {
            CheckDisposed();

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
            CheckDisposed();

            if (offset is null)
            {
                throw new ArgumentNullException(nameof(offset));
            }

            _consumer.Seek(offset);
        }

        public IEnumerable<TopicPartitionOffset> SeekBatch(IEnumerable<ConsumeResult<TKey, TValue>> consumeResults)
        {
            CheckDisposed();

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
            CheckDisposed();

            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty, or contain null values.", nameof(partitions));
            }

            var pausingPartitions = partitions.Where(partition => partition is not null).Distinct();

            _consumer.Pause(pausingPartitions);
        }

        public void Resume(IEnumerable<TopicPartition> partitions)
        {
            CheckDisposed();

            if (partitions is null || !partitions.Any(partition => partition is not null))
            {
                throw new ArgumentException($"{nameof(partitions)} cannot be null, empty or contain null values.", nameof(partitions));
            }

            var resumingPartitions = partitions.Where(partition => partition is not null).Distinct();

            _consumer.Resume(resumingPartitions);
        }

        public Offset Position(TopicPartition partition)
        {
            CheckDisposed();

            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition));
            }

            var offsetPosition = _consumer.Position(partition);

            return offsetPosition;
        }

        public List<TopicPartitionOffset> OffsetsForTimes(IEnumerable<TopicPartitionTimestamp> timestamps, TimeSpan timeout)
        {
            CheckDisposed();

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
            CheckDisposed();

            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition));
            }

            var watermarkOffsets = _consumer.GetWatermarkOffsets(partition);

            return watermarkOffsets;
        }

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition partition, TimeSpan timeout)
        {
            CheckDisposed();

            if (partition is null)
            {
                throw new ArgumentNullException(nameof(partition));
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
            CheckDisposed();

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
            CheckDisposed();

            _consumer.Close();
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

                var messageId = consumeResult.Message!.GetId(_options.MessageIdHandler);

                if (_options.ConsumerConfig!.CommitAfterConsuming)
                {
                    try
                    {
                        _consumer.Commit(consumeResult);
                    }
                    catch (Exception ex)
                    {
                        switch (ex)
                        {
                            case KafkaException kafkaException:
                                {
                                    _logger.LogMessageCommitFailure(
                                        kafkaException,
                                        messageId,
                                        consumeResult.Topic,
                                        consumeResult.Partition,
                                        consumeResult.Offset,
                                        kafkaException.Error);
                                }
                                break;
                            default:
                                {
                                    _logger.LogMessageCommitFailure(
                                        ex,
                                        messageId,
                                        consumeResult.Topic,
                                        consumeResult.Partition,
                                        consumeResult.Offset);
                                }
                                break;
                        }
                    }
                }

                _logger.LogMessageConsumptionSuccess(
                    messageId,
                    consumeResult.Topic,
                    consumeResult.Partition,
                    consumeResult.Offset);

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
                    if (consumeResult.Message is null)
                    {
                        _logger.LogMessageConsumptionInterceptionFailure(
                            ex,
                            consumeResult.Topic,
                            consumeResult.Partition,
                            consumeResult.Offset);
                    }
                    else
                    {
                        var messageId = consumeResult.Message!.GetId(_options.MessageIdHandler);

                        _logger.LogMessageConsumptionInterceptionFailure(
                            ex,
                            messageId,
                            consumeResult.Topic,
                            consumeResult.Partition,
                            consumeResult.Offset);
                    }

                    if (_options.ConsumerConfig!.EnableInterceptorExceptionPropagation)
                    {
                        throw;
                    }
                }
            }
        }

        private void OnConsumeRetry(Exception exception, int retryAttempt)
        {
            switch (exception)
            {
                case ConsumeException consumeException:
                    {
                        _logger.LogMessageConsumptionRetryFailure(
                            exception,
                            retryAttempt,
                            consumeException.ConsumerRecord!.Topic,
                            consumeException.ConsumerRecord!.Partition,
                            consumeException.ConsumerRecord!.Offset,
                            consumeException.Error);
                    }
                    break;
                default:
                    {
                        _logger.LogMessageConsumptionRetryFailure(
                            exception,
                            retryAttempt,
                            _consumer.GetCurrentTopics());
                    }
                    break;
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
                consumeException.ConsumerRecord!.Offset,
                consumeException.Error);

            activity?.SetStatus(ActivityStatusCode.Error);

            _options.DiagnosticsManager!.Enrich(activity, consumeException, _options.ConsumerConfig);
        }

        private void ProduceDeadLetterMessage(ConsumeResult<byte[], byte[]> consumeResult, Error error)
        {
            if (_options.ConsumerConfig!.EnableDeadLetterTopic)
            {
                var producerConfig = _options.DeadLetterProducer!.Options!.ProducerConfig;

                var deadLetterTopic = !string.IsNullOrWhiteSpace(producerConfig!.DefaultTopic)
                    ? producerConfig!.DefaultTopic
                    : $"{consumeResult.Topic}{KafkaProducerConstants.DeadLetterTopicSuffix}";

                var message = new Message<byte[], KafkaMetadataMessage>
                {
                    Key = consumeResult.Message!.Key,
                    Value = new KafkaMetadataMessage
                    {
                        Id = Guid.NewGuid(),
                        SourceTopic = consumeResult.Topic,
                        SourceGroupId = _options.ConsumerConfig!.GroupId,
                        SourcePartition = consumeResult.Partition,
                        SourceOffset = consumeResult.Offset,
                        SourceKey = consumeResult.Message!.Key,
                        SourceMessage = consumeResult.Message!.Value,
                        SourceKeyType = typeof(TKey).AssemblyQualifiedName,
                        SourceMessageType = typeof(TValue).AssemblyQualifiedName,
                        ErrorCode = error?.Code ?? ErrorCode.Unknown,
                        Reason = error?.Reason ?? ErrorCode.Unknown.GetReason()
                    },
                    Headers = consumeResult.Message!.Headers
                };

                try
                {
                    _options.DeadLetterProducer!.Produce(deadLetterTopic, message);
                }
                catch (Exception ex)
                {
                    _logger.LogDeadLetterProductionFailure(
                        ex,
                        consumeResult.Topic,
                        consumeResult.Partition,
                        consumeResult.Offset,
                        deadLetterTopic);
                }

                try
                {
                    _consumer.Commit(
                    [
                        new TopicPartitionOffset(consumeResult.TopicPartition, consumeResult.Offset + 1)
                    ]);
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case KafkaException kafkaException:
                            {
                                _logger.LogMessageCommitFailure(
                                    kafkaException,
                                    consumeResult.Topic,
                                    consumeResult.Partition,
                                    consumeResult.Offset,
                                    kafkaException.Error);
                            }
                            break;
                        default:
                            {
                                _logger.LogMessageCommitFailure(
                                    ex,
                                    consumeResult.Topic,
                                    consumeResult.Partition,
                                    consumeResult.Offset);
                            }
                            break;
                    }
                }
            }
        }

        private Activity StartActivity(string topic, IDictionary<string, string> headers)
        {
            var activityName = $"{topic} {OperationNames.ReceiveOperation}";

            var activity = _options.DiagnosticsManager!.StartConsumerActivity(activityName, headers);

            return activity;
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(_options.ConsumerType!.ExtractTypeName());
        }

        #region IDisposable Members

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _consumer?.Close();
                    _consumer?.Dispose();

                    _options.DeadLetterProducer?.Dispose();
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
