namespace Confluent.Kafka.Core.Mapping.Internal
{
    internal interface IMapper<TDestination>
    {
        TDestination Map(params object[] args);
    }
}
