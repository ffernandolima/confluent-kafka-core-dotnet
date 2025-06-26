using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf.Internal;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Serialization
{
    using System.Text;

    public sealed class SchemaRegistryProtobufSerializerTests : IDisposable
    {
        private const string SchemaRegistryUrl = "localhost:8081";

        private readonly Encoding _encoding;
        private readonly SerializationContext _context;
        private readonly ISchemaRegistryClient _schemaRegistryClient;
        private readonly SchemaRegistryProtobufSerializer<ProtobufMessage> _serializer;

        public SchemaRegistryProtobufSerializerTests()
        {
            _encoding = EncodingFactory.Instance.CreateDefault();

            _context = new SerializationContext(MessageComponentType.Value, "test-protobuf-topic");

            _schemaRegistryClient = CreateSchemaRegistryClient();

            _serializer = new SchemaRegistryProtobufSerializer<ProtobufMessage>(
                _schemaRegistryClient,
                new ProtobufSerializerConfig { AutoRegisterSchemas = true });
        }

        private static ISchemaRegistryClient CreateSchemaRegistryClient()
        {
            var config = new SchemaRegistryConfig
            {
                Url = SchemaRegistryUrl
            };

            return new CachedSchemaRegistryClient(config);
        }

        public void Dispose()
        {
            _schemaRegistryClient?.Dispose();

            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task SerializeAsync_ReturnsSerializedBytes()
        {
            // Arrange
            var message = new ProtobufMessage { Id = 1, Content = "Test content" };

            // Act
            var result = await _serializer.SerializeAsync(message, _context);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public async Task SerializeAsync_NullData_ReturnsNull()
        {
            // Act
            var result = await _serializer.SerializeAsync(null, _context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeserializeAsync_ReturnsDeserializedObject()
        {
            // Arrange
            var message = new ProtobufMessage { Id = 2, Content = "Test content" };
            var serializeResult = await _serializer.SerializeAsync(message, _context);

            // Act
            var deserializeResult = await _serializer.DeserializeAsync(serializeResult, false, _context);

            // Assert
            Assert.NotNull(deserializeResult);
            Assert.Equal(message.Id, deserializeResult.Id);
            Assert.Equal(message.Content, deserializeResult.Content);
        }

        [Fact]
        public async Task Deserialize_NullData_ReturnsNull_WhenIsNullFlagIsTrue()
        {
            // Act
            var result = await _serializer.DeserializeAsync(null, true, _context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Deserialize_NullData_ThrowsIndexOutOfRangeException_WhenIsNullFlagIsFalse()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<IndexOutOfRangeException>(() => _serializer.DeserializeAsync(null, false, _context));

            Assert.Contains("Index was outside the bounds of the array.", exception.Message);
        }

        [Fact]
        public async Task Deserialize_EmptyData_ThrowsIndexOutOfRangeException()
        {
            // Arrange
            var emptyData = Array.Empty<byte>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<IndexOutOfRangeException>(() => _serializer.DeserializeAsync(emptyData, false, _context));

            Assert.Contains("Index was outside the bounds of the array.", exception.Message);
        }

        [Fact]
        public async Task DeserializeAsync_InvalidData_ThrowsInvalidDataException()
        {
            // Arrange
            var invalidData = _encoding.GetBytes("invalid data");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidDataException>(() => _serializer.DeserializeAsync(invalidData, false, _context));

            Assert.Contains("Invalid magic byte: 105", exception.Message);
        }
    }
}
