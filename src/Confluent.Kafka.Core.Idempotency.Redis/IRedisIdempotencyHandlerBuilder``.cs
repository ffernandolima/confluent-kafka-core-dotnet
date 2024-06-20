﻿using System;

namespace Confluent.Kafka.Core.Idempotency.Redis
{
    public interface IRedisIdempotencyHandlerBuilder<TKey, TValue>
    {
        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithRedis(
            Action<IConfigurationOptionsBuilder> configureOptions);

        IRedisIdempotencyHandlerBuilder<TKey, TValue> WithHandlerOptions(
            Action<IRedisIdempotencyHandlerOptionsBuilder<TKey, TValue>> configureOptions);
    }
}
