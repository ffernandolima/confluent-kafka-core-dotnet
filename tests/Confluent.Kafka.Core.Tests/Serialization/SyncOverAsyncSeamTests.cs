using Confluent.Kafka.Core.Serialization.Internal;
using Confluent.Kafka.SyncOverAsync;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Serialization
{
    /// <summary>
    /// GetInnerSerializer/GetInnerDeserializer reach into private members of Confluent's sync-over-async wrappers by hard-coded name.
    /// Those members carry no compatibility guarantee, and because the lookup is null-conditional a rename degrades to a silent null rather than an exception. 
    /// These tests turn that silent failure into a build failure.
    /// </summary>
    public sealed class SyncOverAsyncSeamTests
    {
        [Fact]
        public void GetInnerSerializer_UnwrapsTheAsyncSerializer()
        {
            // Arrange
            var asyncSerializer = new StubAsyncSerializer();
            var syncOverAsyncSerializer = (SyncOverAsyncSerializer<StubMessage>)asyncSerializer.AsSyncOverAsync();

            // Act
            var result = syncOverAsyncSerializer.GetInnerSerializer();

            // Assert
            Assert.NotNull(result);
            Assert.Same(asyncSerializer, result);
        }

        [Fact]
        public void GetInnerDeserializer_UnwrapsTheAsyncDeserializer()
        {
            // Arrange
            var asyncDeserializer = new StubAsyncDeserializer();
            var syncOverAsyncDeserializer = (SyncOverAsyncDeserializer<StubMessage>)asyncDeserializer.AsSyncOverAsync();

            // Act
            var result = syncOverAsyncDeserializer.GetInnerDeserializer();

            // Assert
            Assert.NotNull(result);
            Assert.Same(asyncDeserializer, result);
        }

        [Fact]
        public void GetInnerSerializer_NullSource_ReturnsNull()
        {
            // Act
            var result = ((SyncOverAsyncSerializer<StubMessage>)null).GetInnerSerializer();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetInnerDeserializer_NullSource_ReturnsNull()
        {
            // Act
            var result = ((SyncOverAsyncDeserializer<StubMessage>)null).GetInnerDeserializer();

            // Assert
            Assert.Null(result);
        }

        #region Stubs

        public sealed class StubMessage
        {
            public int Id { get; set; }
        }

        private sealed class StubAsyncSerializer : IAsyncSerializer<StubMessage>
        {
            public Task<byte[]> SerializeAsync(StubMessage data, SerializationContext context)
                => Task.FromResult(Array.Empty<byte>());
        }

        private sealed class StubAsyncDeserializer : IAsyncDeserializer<StubMessage>
        {
            public Task<StubMessage> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
                => Task.FromResult(new StubMessage());
        }

        #endregion Stubs
    }
}
