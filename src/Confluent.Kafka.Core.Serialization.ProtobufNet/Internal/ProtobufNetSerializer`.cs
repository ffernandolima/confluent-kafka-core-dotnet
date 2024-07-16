using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;
using System.Linq;

namespace Confluent.Kafka.Core.Serialization.ProtobufNet.Internal
{
    internal sealed class ProtobufNetSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        private readonly ProtobufNetSerializerOptions _options;

        public ProtobufNetSerializer(ProtobufNetSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            TryMapAutomatically();
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            if (data is null)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();

            Serializer.Serialize(memoryStream, data);

            var result = memoryStream.ToArray();

            return result;
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return default;
            }

            var result = Serializer.Deserialize<T>(data);

            return result;
        }

        private void TryMapAutomatically()
        {
            if (!_options.AutomaticRuntimeMap)
            {
                return;
            }

            var sourceType = typeof(T);

            if (!RuntimeTypeModel.Default.IsDefined(sourceType))
            {
                var metaType = RuntimeTypeModel.Default.Add(sourceType, applyDefaultBehaviour: false);

                var fields = sourceType.GetFields();

                var fieldIdx = 0;

                foreach (var field in fields.OrderBy(field => field.Name))
                {
                    metaType.Add(++fieldIdx, field.Name);
                }

                var properties = sourceType.GetProperties();

                var propertyIdx = 0;

                foreach (var property in properties.OrderBy(property => property.Name))
                {
                    metaType.Add(++propertyIdx, property.Name);
                }
            }
        }
    }
}
