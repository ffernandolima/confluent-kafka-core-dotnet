using Confluent.Kafka.Core.Encoding;
using System;
using System.Text.Json;

namespace Confluent.Kafka.Core.Serialization.JsonCore.Internal
{
    using System.Text;

    internal sealed class JsonCoreSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        private readonly Encoding _encoding;
        private readonly JsonSerializerOptions _options;

        public JsonCoreSerializer(JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _encoding = EncodingFactory.Instance.CreateDefault();
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            if (data is null)
            {
                return null;
            }

            var json = JsonSerializer.Serialize(data, data.GetType(), _options);

            var result = _encoding.GetBytes(json);

            return result;
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return default;
            }

#if NETSTANDARD2_0
            var json = _encoding.GetString(data.ToArray());
#else
            var json = _encoding.GetString(data);
#endif
            var result = JsonSerializer.Deserialize<T>(json, _options);

            return result;
        }
    }
}
