using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Internal
{
    internal static class DictionaryExtensions
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} cannot be null.");
            }

            if (dictionary.IsReadOnly)
            {
                throw new NotSupportedException($"{nameof(dictionary)} cannot be read-only.");
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key), $"{nameof(key)} cannot be null.");
            }

            dictionary[key] = value;
        }
    }
}
