using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
#if NET8_0_OR_GREATER
using NJsonSchema.NewtonsoftJson.Generation;
#else
using NJsonSchema.Generation;
#endif
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
#if NET8_0_OR_GREATER
            NewtonsoftJsonSchemaGeneratorSettings schemaGeneratorSettings = null,
#else
            JsonSchemaGeneratorSettings schemaGeneratorSettings = null,
#endif
            RuleRegistry ruleRegistry = null)
        {
            if (schemaRegistryClient is null)
            {
                throw new ArgumentNullException(nameof(schemaRegistryClient));
            }

            if (schema is not null)
            {
                _serializer = new JsonSerializer<T>(schemaRegistryClient, schema, serializerConfig, schemaGeneratorSettings, ruleRegistry);
                _deserializer = new JsonDeserializer<T>(schemaRegistryClient, schema, deserializerConfig, schemaGeneratorSettings);
            }
            else
            {
                _serializer = new JsonSerializer<T>(schemaRegistryClient, serializerConfig, schemaGeneratorSettings, ruleRegistry);
                _deserializer = new JsonDeserializer<T>(schemaRegistryClient, deserializerConfig, schemaGeneratorSettings, ruleRegistry);
            }
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
