using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Google.Protobuf;
using System;
using System.Collections.Generic;
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
            ProtobufDeserializerConfig deserializerConfig = null,
            IList<IRuleExecutor> ruleExecutors = null)
        {
            if (schemaRegistryClient is null)
            {
                throw new ArgumentNullException(nameof(schemaRegistryClient));
            }

            _serializer = new ProtobufSerializer<T>(schemaRegistryClient, serializerConfig, ruleExecutors);
            _deserializer = new ProtobufDeserializer<T>(schemaRegistryClient, deserializerConfig, ruleExecutors);
        }

        public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
        {
            var result = await _serializer.SerializeAsync(data, context)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
        {
            var result = await _deserializer.DeserializeAsync(data, isNull, context)
                .ConfigureAwait(false);

            return result;
        }
    }
}
