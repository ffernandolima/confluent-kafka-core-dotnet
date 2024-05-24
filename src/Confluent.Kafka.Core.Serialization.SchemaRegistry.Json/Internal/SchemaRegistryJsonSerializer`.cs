using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using NJsonSchema.Generation;
using System;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Json.Internal
{
    internal sealed class SchemaRegistryJsonSerializer<T> : IAsyncSerializer<T>, IAsyncDeserializer<T> where T : class
    {
        private readonly JsonSerializer<T> _serializer;
        private readonly JsonDeserializer<T> _deserializer;

        public SchemaRegistryJsonSerializer(
            ISchemaRegistryClient schemaRegistryClient,
            Schema schema = null,
            JsonSerializerConfig serializerConfig = null,
            JsonDeserializerConfig deserializerConfig = null,
            JsonSchemaGeneratorSettings schemaGeneratorSettings = null)
        {
            if (schemaRegistryClient is null)
            {
                throw new ArgumentNullException(nameof(schemaRegistryClient), $"{nameof(schemaRegistryClient)} cannot be null.");
            }

            if (schema is not null)
            {
                _serializer = new JsonSerializer<T>(schemaRegistryClient, schema, serializerConfig, schemaGeneratorSettings);
                _deserializer = new JsonDeserializer<T>(schemaRegistryClient, schema, deserializerConfig, schemaGeneratorSettings);
            }
            else
            {
                _serializer = new JsonSerializer<T>(schemaRegistryClient, serializerConfig, schemaGeneratorSettings);
                _deserializer = new JsonDeserializer<T>(deserializerConfig, schemaGeneratorSettings);
            }
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
