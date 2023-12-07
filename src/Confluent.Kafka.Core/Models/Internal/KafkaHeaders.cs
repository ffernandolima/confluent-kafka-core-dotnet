using Confluent.Kafka.Core.Encoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Models.Internal
{
    using System.Text;

    internal sealed class KafkaHeaders : IDictionary<string, string>
    {
        private readonly Headers _headers;
        private readonly Encoding _encoding;

        public string this[string key]
        {
            get => _encoding.GetString(_headers.GetLastBytes(key));
            set
            {
                _headers.Remove(key);
                _headers.Add(key, _encoding.GetBytes(value));
            }
        }

        public ICollection<string> Keys
            => _headers.Select(header => header.Key)
                       .Distinct()
                       .ToList()
                       .AsReadOnly();

        public ICollection<string> Values
            => _headers.Select(header => header.Key)
                       .Distinct()
                       .Select(key => _encoding.GetString(_headers.GetLastBytes(key)))
                       .ToList()
                       .AsReadOnly();

        public int Count
            => _headers.Count;

        public bool IsReadOnly
            => false;

        public KafkaHeaders(Headers headers, Encoding encoding = null)
        {
            _headers = headers ?? throw new ArgumentNullException(nameof(headers), $"{nameof(headers)} cannot be null.");
            _encoding = encoding ?? IEncodingFactory.Create();
        }

        public void Add(string key, string value)
            => _headers.Add(key, _encoding.GetBytes(value));

        public void Add(KeyValuePair<string, string> item)
            => _headers.Add(new Header(item.Key, _encoding.GetBytes(item.Value)));

        public void Clear()
        {
            while (_headers.Count > 0)
            {
                _headers.Remove(_headers[0].Key);
            }
        }

        public bool Contains(KeyValuePair<string, string> item)
            => _headers.Any(header => header.Key == item.Key && _encoding.GetString(header.GetValueBytes()) == item.Value);

        public bool ContainsKey(string key)
            => _headers.Any(header => header.Key == key);

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(
                    nameof(array), $"{nameof(array)} cannot be null.");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(arrayIndex), $"{nameof(arrayIndex)} cannot be less than zero.");
            }

            if (arrayIndex >= _headers.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(arrayIndex), $"{nameof(arrayIndex)} cannot be greater than or equal to {_headers.Count}.");
            }

            for (var idx = arrayIndex; idx < _headers.Count; idx++)
            {
                var header = _headers[idx];
                array[idx - arrayIndex] = new KeyValuePair<string, string>(header.Key, _encoding.GetString(header.GetValueBytes()));
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => _headers.Select(header => new KeyValuePair<string, string>(header.Key, _encoding.GetString(header.GetValueBytes())))
                       .GetEnumerator();

        public bool Remove(string key)
        {
            if (_headers.TryGetLastBytes(key, out _))
            {
                _headers.Remove(key);
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            if (!_headers.Any(header => header.Key == item.Key &&
                _encoding.GetString(header.GetValueBytes()) == item.Value))
            {
                return false;
            }

            var headers = _headers.Where(header => header.Key == item.Key &&
                _encoding.GetString(header.GetValueBytes()) != item.Value).ToList();

            _headers.Remove(item.Key);

            foreach (var header in headers)
            {
                _headers.Add(header.Key, header.GetValueBytes());
            }

            return true;
        }

        public bool TryGetValue(string key, out string value)
        {
            if (_headers.TryGetLastBytes(key, out var lastBytes))
            {
                value = _encoding.GetString(lastBytes);
                return true;
            }

            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
