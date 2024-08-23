namespace Confluent.Kafka.Core.Mapping
{
    internal interface IMapper<TDestination>
    {
        TDestination Map(params object[] args);
    }
}
