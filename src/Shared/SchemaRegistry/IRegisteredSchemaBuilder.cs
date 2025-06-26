namespace Confluent.Kafka.Core.Serialization.SchemaRegistry
{
    public interface IRegisteredSchemaBuilder : ISchemaBuilder<IRegisteredSchemaBuilder>
    {
        IRegisteredSchemaBuilder WithSubject(string subject);

        IRegisteredSchemaBuilder WithVersion(int version);

        IRegisteredSchemaBuilder WithId(int id);

        IRegisteredSchemaBuilder WithGuid(string guid);
    }
}
