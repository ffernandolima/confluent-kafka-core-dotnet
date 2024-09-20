using Confluent.Kafka.Core.Encoding;
using Confluent.Kafka.Core.Serialization.ProtobufNet;
using Confluent.Kafka.Core.Serialization.ProtobufNet.Internal;
using ProtoBuf;
using System;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Serialization
{
    using System.Text;

    public sealed class ProtobufNetSerializerTests
    {
        private readonly Encoding _encoding;
        private readonly SerializationContext _context;
        private readonly ProtobufNetSerializer<ProtobufMessage> _serializer;

        public ProtobufNetSerializerTests()
        {
            _encoding = EncodingFactory.Instance.CreateDefault();

            _context = SerializationContext.Empty;

            var options = new ProtobufNetSerializerOptions
            {
                AutomaticRuntimeMap = true
            };

            _serializer = new ProtobufNetSerializer<ProtobufMessage>(options);
        }

        #region Stubs

        [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
        public class ProtobufMessage
        {
            public int Id { get; set; }
            public string Content { get; set; }
        }

        #endregion Stubs

        [Fact]
        public void Serialize_ValidObject_ReturnsSerializedBytes()
        {
            // Arrange
            var message = new ProtobufMessage { Id = 1, Content = "Test message" };

            // Act
            var result = _serializer.Serialize(message, _context);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
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
        public void Deserialize_ValidData_ReturnsOriginalObject()
        {
            // Arrange
            var message = new ProtobufMessage { Id = 2, Content = "Test message" };
            var serializeResult = _serializer.Serialize(message, _context);

            // Act
            var deserializeResult = _serializer.Deserialize(serializeResult, false, _context);

            // Assert
            Assert.NotNull(deserializeResult);
            Assert.Equal(message.Id, deserializeResult.Id);
            Assert.Equal(message.Content, deserializeResult.Content);
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
        public void Deserialize_NullData_ReturnsDefaultObject_WhenIsNullFlagIsFalse()
        {
            // Act
            var result = _serializer.Deserialize(null, false, _context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Null(result.Content);
        }

        [Fact]
        public void Deserialize_EmptyData_ReturnsDefaultObject()
        {
            // Arrange
            var emptyData = Array.Empty<byte>();

            // Act
            var result = _serializer.Deserialize(emptyData, false, _context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Null(result.Content);
        }

        [Fact]
        public void Deserialize_InvalidData_ThrowsProtoException()
        {
            // Arrange
            var invalidJsonData = "invalid json";
            var dataBytes = _encoding.GetBytes(invalidJsonData);

            // Act & Assert
            var exception = Assert.Throws<ProtoException>(() => _serializer.Deserialize(dataBytes, false, _context));

            Assert.Contains("Invalid wire-type", exception.Message);
        }
    }
}
