using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Google.Protobuf;
using System;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal
{
    internal sealed class SchemaRegistryProtobufSerializer<T> : IAsyncSerializer<T>, IAsyncDeserializer<T>
        where T : class, IMessage<T>, new()
    {
        private readonly ProtobufSerializer<T> _serializer;
        private readonly ProtobufDeserializer<T> _deserializer;

        public SchemaRegistryProtobufSerializer(
            ISchemaRegistryClient schemaRegistryClient,
            ProtobufSerializerConfig serializerConfig = null,
            ProtobufDeserializerConfig deserializerConfig = null)
        {
            if (schemaRegistryClient is null)
            {
                throw new ArgumentNullException(nameof(schemaRegistryClient), $"{nameof(schemaRegistryClient)} cannot be null.");
            }

            _serializer = new ProtobufSerializer<T>(schemaRegistryClient, serializerConfig);
            _deserializer = new ProtobufDeserializer<T>(deserializerConfig);
        }

        public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
        {
            if (data is null)
            {
                return null;
            }

            var result = await _serializer.SerializeAsync(data, context)
                .ConfigureAwait(continueOnCapturedContext: false);

            return result;
        }

        public async Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return null;
            }

            var result = await _deserializer.DeserializeAsync(data, isNull, context)
                .ConfigureAwait(continueOnCapturedContext: false);

            return result;
        }
    }
}
