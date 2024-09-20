using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Serialization.NewtonsoftJson.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Serialization
{
    using System.Text;

    public sealed class NewtonsoftJsonSerializerTests
    {
        private readonly Encoding _encoding;
        private readonly SerializationContext _context;
        private readonly NewtonsoftJsonSerializer<JsonMessage> _serializer;

        public NewtonsoftJsonSerializerTests()
        {
            _encoding = EncodingFactory.Instance.CreateDefault();

            _context = SerializationContext.Empty;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            _serializer = new NewtonsoftJsonSerializer<JsonMessage>(settings);
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
        public void Deserialize_NullData_ReturnsNull_WhenIsNullFlagIsFalse()
        {
            // Act
            var result = _serializer.Deserialize(null, false, _context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_EmptyData_ReturnsNull()
        {
            // Arrange
            var emptyData = Array.Empty<byte>();

            // Act & Assert
            var result = _serializer.Deserialize(emptyData, false, _context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_InvalidJson_ThrowsJsonReaderException()
        {
            // Arrange
            var invalidJsonData = "invalid json";
            var dataBytes = _encoding.GetBytes(invalidJsonData);

            // Act & Assert
            var exception = Assert.Throws<JsonReaderException>(() => _serializer.Deserialize(dataBytes, false, _context));

            Assert.Contains("Unexpected character encountered while parsing value", exception.Message);
        }
    }
}
