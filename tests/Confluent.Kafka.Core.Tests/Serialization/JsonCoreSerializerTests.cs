using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Serialization.JsonCore.Internal;
using System;
using System.Text.Json;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Serialization
{
    using System.Text;

    public class JsonCoreSerializerTests
    {
        private readonly Encoding _encoding;
        private readonly SerializationContext _context;
        private readonly JsonCoreSerializer<JsonMessage> _serializer;

        public JsonCoreSerializerTests()
        {
            _encoding = EncodingFactory.Instance.CreateDefault();

            _context = SerializationContext.Empty;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            _serializer = new JsonCoreSerializer<JsonMessage>(options);
        }

        #region Stubs

        public class JsonMessage
        {
            public int Id { get; set; }
            public string Content { get; set; }
        }

        #endregion Stubs

        [Fact]
        public void Serialize_ValidObject_ReturnsSerializedBytes()
        {
            // Arrange
            var message = new JsonMessage { Id = 1, Content = "Test message" };

            // Act
            var result = _serializer.Serialize(message, _context);

            // Assert
            var jsonString = _encoding.GetString(result);

            Assert.NotNull(result);
            Assert.Contains("\"id\": 1", jsonString);
            Assert.Contains("\"content\": \"Test message\"", jsonString);
        }

        [Fact]
        public void Serialize_NullObject_ReturnsNull()
        {
            // Act
            var result = _serializer.Serialize(null, _context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_ValidJson_ReturnsDeserializedObject()
        {
            // Arrange
            var jsonData = "{\"id\":1,\"content\":\"Test message\"}";
            var dataBytes = _encoding.GetBytes(jsonData);

            // Act
            var result = _serializer.Deserialize(dataBytes, false, _context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test message", result.Content);
        }

        [Fact]
        public void Deserialize_NullData_ReturnsNull_WhenIsNullFlagIsTrue()
        {
            // Act
            var result = _serializer.Deserialize(null, true, _context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_NullData_ThrowsJsonException_WhenIsNullFlagIsFalse()
        {
            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => _serializer.Deserialize(null, false, _context));

            Assert.Contains("The input does not contain any JSON tokens", exception.Message);
        }

        [Fact]
        public void Deserialize_EmptyData_ThrowsJsonException()
        {
            // Arrange
            var emptyData = Array.Empty<byte>();

            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => _serializer.Deserialize(emptyData, false, _context));

            Assert.Contains("The input does not contain any JSON tokens", exception.Message);
        }

        [Fact]
        public void Deserialize_InvalidJson_ThrowsJsonException()
        {
            // Arrange
            var invalidJsonData = "invalid json";
            var dataBytes = _encoding.GetBytes(invalidJsonData);

            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => _serializer.Deserialize(dataBytes, false, _context));

            Assert.Contains("invalid start of a value", exception.Message);
        }
    }
}
