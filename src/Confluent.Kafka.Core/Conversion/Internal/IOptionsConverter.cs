namespace Confluent.Kafka.Core.Conversion.Internal
{
    internal interface IOptionsConverter<TOptions>
    {
        TOptions ToOptions();
    }
}
