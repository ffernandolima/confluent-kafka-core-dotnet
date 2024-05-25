using Confluent.Kafka.Core.Encoding;
using Newtonsoft.Json;
using System;

namespace Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal
{
    using System.Text;

    internal sealed class NewtonsoftJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        private readonly Encoding _encoding;
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer(JsonSerializerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), $"{nameof(settings)} cannot be null.");
            _encoding = EncodingFactory.CreateDefault();
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            if (data is null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(data, data.GetType(), _settings);

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
            var result = JsonConvert.DeserializeObject<T>(json, _settings);

            return result;
        }
    }
}
