﻿using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Threading.Tasks;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro.Internal
{
    internal sealed class SchemaRegistryAvroSerializer<T> : IAsyncSerializer<T>, IAsyncDeserializer<T>
    {
        private readonly AvroSerializer<T> _serializer;
        private readonly AvroDeserializer<T> _deserializer;

        public SchemaRegistryAvroSerializer(
           ISchemaRegistryClient schemaRegistryClient,
           AvroSerializerConfig serializerConfig = null,
           AvroDeserializerConfig deserializerConfig = null)
        {
            if (schemaRegistryClient is null)
            {
                throw new ArgumentNullException(nameof(schemaRegistryClient), $"{nameof(schemaRegistryClient)} cannot be null.");
            }

            _serializer = new AvroSerializer<T>(schemaRegistryClient, serializerConfig);
            _deserializer = new AvroDeserializer<T>(schemaRegistryClient, deserializerConfig);
        }

        public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
        {
            var result = await _serializer.SerializeAsync(data, context)
                .ConfigureAwait(continueOnCapturedContext: false);

            return result;
        }

        public async Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
        {
            var result = await _deserializer.DeserializeAsync(data, isNull, context)
                .ConfigureAwait(continueOnCapturedContext: false);

            return result;
        }
    }
}