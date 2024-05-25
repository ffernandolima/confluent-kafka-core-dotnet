using Confluent.SchemaRegistry;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface ISchemaBuilder<TBuilder> where TBuilder : ISchemaBuilder<TBuilder>
    {
        TBuilder WithSchemaString(string schemaString);

        TBuilder WithSchemaType(SchemaType schemaType);

        TBuilder WithSchemaReferences(List<SchemaReference> schemaReferences);
    }
}
