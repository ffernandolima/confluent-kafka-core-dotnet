﻿using System;
using System.Linq;

namespace Confluent.Kafka.Core.Producer
{
    public static class ProduceExceptionExtensions
    {
        private static readonly ErrorCode[] SerializationCodes = new[]
        {
            ErrorCode.Local_KeySerialization,
            ErrorCode.Local_ValueSerialization
        };

        public static bool IsSerializationException<TKey, TValue>(this ProduceException<TKey, TValue> produceException)
        {
            if (produceException is null)
            {
                throw new ArgumentNullException(nameof(produceException), $"{nameof(produceException)} cannot be null.");
            }

            var isSerializationException = SerializationCodes.Contains(produceException.Error!.Code);

            return isSerializationException;
        }
    }
}