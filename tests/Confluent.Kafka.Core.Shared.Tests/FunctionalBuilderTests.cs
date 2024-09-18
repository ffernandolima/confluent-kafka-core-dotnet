using Confluent.Kafka.Core.Shared.Tests.Stubs;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Confluent.Kafka.Core.Shared.Tests
{
    public class FunctionalBuilderTests
    {
        [Fact]
        public void Build_ShouldCreateSubjectUsingDefaultFactory()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var builder = new TestFunctionalBuilder(mockConfiguration.Object);

            // Act
            var result = builder.Build();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestSubject>(result);
            Assert.Null(result.SomeProperty);
        }

        [Fact]
        public void AppendParameter_ShouldAddParameter_WhenActionProvided()
        {
            // Arrange
            var builder = new TestFunctionalBuilder()
                .WithParameter("TestValue");

            // Act
            var result = builder.Build();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestSubject>(result);
            Assert.Equal("TestValue", result.SomeProperty);
        }

        [Fact]
        public void AppendAction_ShouldAppendAction_WhenActionProvided()
        {
            // Arrange
            var builder = new TestFunctionalBuilder()
                .WithProperty("TestValue");

            // Act
            var result = builder.Build();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestSubject>(result);
            Assert.Equal("TestValue", result.SomeProperty);
        }

        [Fact]
        public void Clear_ShouldResetParametersAndFunctions()
        {
            // Arrange
            var builder = new TestFunctionalBuilder()
                .WithParameter("TestValue")
                .WithProperty("UpdatedValue");

            // Act
            builder.Clear();
            var result = builder.Build();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestSubject>(result);
            Assert.NotEqual("TestValue", result.SomeProperty);
            Assert.NotEqual("UpdatedValue", result.SomeProperty);
        }

        [Fact]
        public void Dispose_ShouldPreventFurtherOperations()
        {
            // Arrange
            var builder = new TestFunctionalBuilder();

            // Act & Assert
            builder.Dispose();
            Assert.Throws<ObjectDisposedException>(builder.Build);
        }

        [Fact]
        public void Bind_ShouldRetrieveConfigurationSectionAndBindToSubject()
        {
            // Arrange
            Dictionary<string, string> inMemoryData = new()
            {
                {"TestSection:SomeProperty", "TestValue"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryData)
                .Build();

            var builder = new TestFunctionalBuilder(configuration);

            // Act
            builder.FromConfiguration("TestSection");
            var result = builder.Build();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestSubject>(result);
            Assert.Equal("TestValue", result.SomeProperty);
        }
    }
}
