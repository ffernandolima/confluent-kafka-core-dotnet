﻿using System;
using System.Linq;

namespace Confluent.Kafka.Core.Consumer
{
    public static class ConsumeExceptionExtensions
    {
        private static readonly ErrorCode[] DeserializationCodes = new[]
        {
            ErrorCode.Local_KeyDeserialization,
            ErrorCode.Local_ValueDeserialization
        };

        public static bool IsDeserializationException(this ConsumeException consumeException)
        {
            if (consumeException is null)
            {
                throw new ArgumentNullException(nameof(consumeException), $"{nameof(consumeException)} cannot be null.");
            }

            var isDeserializationException = DeserializationCodes.Contains(consumeException.Error!.Code);

            return isDeserializationException;
        }
    }
}