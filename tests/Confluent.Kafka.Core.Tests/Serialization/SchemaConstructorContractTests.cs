using Confluent.SchemaRegistry;
using System;
using System.Collections.Generic;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Serialization
{
    /// <summary>
    /// RegisteredSchemaBuilder and UnregisteredSchemaBuilder construct these types positionally via
    /// Activator.CreateInstance, so a constructor change upstream fails at runtime rather than at
    /// compile time. These tests pin the constructor shapes they depend on.
    /// </summary>
    public sealed class SchemaConstructorContractTests
    {
        // Mirrors RegisteredSchemaBuilder.RegisteredSchemaParameter ordering.
        private static readonly Type[] RegisteredSchemaParameterTypes =
        [
            typeof(string),                // Subject
            typeof(int),                   // Version
            typeof(int),                   // Id
            typeof(string),                // Guid
            typeof(string),                // SchemaString
            typeof(SchemaType),            // SchemaType
            typeof(List<SchemaReference>)  // SchemaReferences
        ];

        // Mirrors UnregisteredSchemaBuilder.UnregisteredSchemaParameter ordering.
        private static readonly Type[] UnregisteredSchemaParameterTypes =
        [
            typeof(string),                // SchemaString
            typeof(List<SchemaReference>), // SchemaReferences
            typeof(SchemaType)             // SchemaType
        ];

        [Fact]
        public void RegisteredSchema_ExposesTheConstructorRegisteredSchemaBuilderDependsOn()
        {
            // Act
            var constructor = typeof(RegisteredSchema).GetConstructor(RegisteredSchemaParameterTypes);

            // Assert
            Assert.True(
                constructor is not null,
                "RegisteredSchema no longer exposes the (subject, version, id, guid, schemaString, schemaType, " +
                "references) constructor that RegisteredSchemaBuilder constructs positionally. Update " +
                "RegisteredSchemaBuilder.RegisteredSchemaParameter to match the new constructor.");
        }

        [Fact]
        public void Schema_ExposesTheConstructorUnregisteredSchemaBuilderDependsOn()
        {
            // Act
            var constructor = typeof(Schema).GetConstructor(UnregisteredSchemaParameterTypes);

            // Assert
            Assert.True(
                constructor is not null,
                "Schema no longer exposes the (schemaString, references, schemaType) constructor that " +
                "UnregisteredSchemaBuilder constructs positionally. Update " +
                "UnregisteredSchemaBuilder.UnregisteredSchemaParameter to match the new constructor.");
        }

        [Fact]
        public void RegisteredSchema_RoundTripsWhenConstructedPositionally()
        {
            // Arrange
            var references = new List<SchemaReference>();
            object[] arguments = ["test-subject", 3, 42, "6f8d2c1e-0000-4a5b-9c3d-1e2f3a4b5c6d", "\"string\"", SchemaType.Avro, references];

            // Act
            var schema = (RegisteredSchema)Activator.CreateInstance(typeof(RegisteredSchema), arguments);

            // Assert
            Assert.Equal("test-subject", schema.Subject);
            Assert.Equal(3, schema.Version);
            Assert.Equal(42, schema.Id);
            Assert.Equal("6f8d2c1e-0000-4a5b-9c3d-1e2f3a4b5c6d", schema.Guid);
            Assert.Equal("\"string\"", schema.SchemaString);
            Assert.Equal(SchemaType.Avro, schema.SchemaType);
            Assert.Same(references, schema.References);
        }

        [Fact]
        public void Schema_RoundTripsWhenConstructedPositionally()
        {
            // Arrange
            var references = new List<SchemaReference>();
            object[] arguments = ["\"string\"", references, SchemaType.Json];

            // Act
            var schema = (Schema)Activator.CreateInstance(typeof(Schema), arguments);

            // Assert
            Assert.Equal("\"string\"", schema.SchemaString);
            Assert.Same(references, schema.References);
            Assert.Equal(SchemaType.Json, schema.SchemaType);
        }
    }
}
